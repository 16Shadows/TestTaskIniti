﻿using Algorithm;
using Simulation.Cameras;
﻿using Simulation.Core;
using Simulation.Roads;
using Simulation.TrafficLights;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Visualization.ViewModels
{
    class CrossroadsViewModel : ViewModelBase, IDisposable
    {
        private readonly SimulationOrchestrator _Simulation;
        
        private readonly Road[] _Crosswalks;
        private readonly RoadViewModel[] _CrosswalkVMs;
        public IReadOnlyList<RoadViewModel> Crosswalks => _CrosswalkVMs;

        private readonly Road[] _Roads;
        private readonly RoadViewModel[] _RoadsVMs;
        public IReadOnlyList<RoadViewModel> Roads => _RoadsVMs;

        private readonly TrafficLightCamera[] _Cameras;
        private readonly TrafficLight[] _TrafficLights;
        private readonly TrafficLightViewModel[] _TrafficLightsVMs;
        public IReadOnlyList<TrafficLightViewModel> TrafficLights => _TrafficLightsVMs;

        private readonly TrafficLightsMediator _TrafficLightsChannel;


        const int crosswalksCount = 8;
        const int roadsCount = 4;
        const int cameraPollingInterval = 100;
        const int intersectionUpdateInterval = 1000;


        public CrossroadsViewModel(IContext context) : base(context)
        {
            //Конфигурация очень чувствительная, изменения могут сломать систему.
            //При изменении ассоциации светофоров (ID) с дорогами/переходами необходимо обновить состояния (CreateStates).
            _Simulation = new SimulationOrchestrator();

            _TrafficLightsChannel = new TrafficLightsMediator();

            _Cameras = new TrafficLightCamera[crosswalksCount + roadsCount];
            _TrafficLights = new TrafficLight[crosswalksCount + roadsCount];
            _TrafficLightsVMs = new TrafficLightViewModel[crosswalksCount + roadsCount];

            
            //Генерируем переходы и ассоциируем камеры 0-7 с ними.
            _Crosswalks = new Road[crosswalksCount];
            _CrosswalkVMs = new RoadViewModel[crosswalksCount];
            for (int i = 0; i < crosswalksCount; i++)
            {
                _Crosswalks[i] = new Road(8000, 12000);
                _Simulation.AddEntity(_Crosswalks[i]);
                _CrosswalkVMs[i] = new RoadViewModel(context, _Crosswalks[i]);

                _Cameras[i] = new TrafficLightCamera(_Crosswalks[i], cameraPollingInterval);
                _Simulation.AddEntity(_Cameras[i]);
            }

            //Генерируем дороги и ассоциируем камеры 8-11 с ними.
            _Roads = new Road[roadsCount];
            _RoadsVMs = new RoadViewModel[roadsCount];
            for (int i = 0; i < roadsCount; i++)
            {
                _Roads[i] = new Road(3000, 5000);
                _Simulation.AddEntity(_Roads[i]);
                _RoadsVMs[i] = new RoadViewModel(context, _Roads[i]);

                _Cameras[crosswalksCount + i] = new TrafficLightCamera(_Roads[i], cameraPollingInterval);
                _Simulation.AddEntity(_Cameras[crosswalksCount + i]);
            }

            //Создаём контролирующий светофор
            _TrafficLights[0] = new TrafficLight(0, _Cameras[0], _TrafficLightsChannel, (controller) => new TrafficLightMasterBrain(controller, CreateStates(), intersectionUpdateInterval));
            _TrafficLightsVMs[0] = new TrafficLightViewModel(context, _Cameras[0], _TrafficLights[0]);
            _Simulation.AddEntity(_TrafficLights[0]);
            _TrafficLightsChannel.Attach(_TrafficLights[0]);

            //Создаём подчинённые светофоры
            for (int i = 1; i < crosswalksCount + roadsCount; i++)
            {
                _TrafficLights[i] = new TrafficLight(i, _Cameras[i], _TrafficLightsChannel, (controller) => new TrafficLightSlaveBrain(controller, 0));
                _Simulation.AddEntity(_TrafficLights[i]);
                _TrafficLightsVMs[i] = new TrafficLightViewModel(context, _Cameras[i], _TrafficLights[i]);
                _TrafficLightsChannel.Attach(_TrafficLights[i]);
            }

            _Simulation.StartSimulation();
        }

        protected IEnumerable<TrafficLightsState> CreateStates()
        {
            //Только пешеходные переходы (светофоры 0-7) открыты.
            TrafficLightsState state = new TrafficLightsState(
                Enumerable.Range(0, crosswalksCount),
                Enumerable.Range(crosswalksCount, roadsCount)
            );
            yield return state;

            //Противонаправленные горизонтальные дороги (8 и 10)
            state = new TrafficLightsState(
                new int[] { 8, 10 },
                new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 9, 11 }
            );
            yield return state;
         
            //Противонаправленные вертикальные дороги (9 и 11)
            state = new TrafficLightsState(
                new int[] { 9, 11 },
                new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 10 }
            );
            yield return state;

            //Дорога и параллельные переходы на противоположной стороне
            //(8 - дорога, 1 и 2 - переходы)
            state = new TrafficLightsState(
                new int[] { 8, 1, 2 },
                new int[] { 0, 5, 6, 3, 4, 7, 9, 10, 11 }
            );
            yield return state;
         
            //(9 - дорога, 3 и 4 - переходы)
            state = new TrafficLightsState(
                new int[] { 9, 3, 4 },
                new int[] { 0, 1, 2, 5, 6, 7, 8, 10, 11 }
            );
            yield return state;

            //(10 - дорога, 5 и 6 - переходы)
            state = new TrafficLightsState(
                new int[] { 10, 5, 6 },
                new int[] { 0, 1, 2, 3, 4, 7, 8, 9, 11 }
            );
            yield return state;

            //(11 - дорога, 7 и 8 - переходы)
            state = new TrafficLightsState(
                new int[] { 11, 7, 8 },
                new int[] { 0, 1, 2, 3, 4, 5, 6, 9, 11 }
            );
            yield return state;

            yield break;
        }

		public void Dispose()
		{
			_Simulation.StopSimulation().Wait();
		}
	}
}
