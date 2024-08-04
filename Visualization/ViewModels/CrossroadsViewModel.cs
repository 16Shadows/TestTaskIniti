using Algorithm;
using Simulation.Cameras;
using Simulation.Core;
using Simulation.Intersection;
using Simulation.Roads;
using Simulation.TrafficLights;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;

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
        private readonly Intersection _Intersection;
        private readonly IntersectionPath[] _Paths;

        protected readonly ObservableCollection<bool> _PathJustPassed;
        public INotifyCollectionChanged PathJustPassed => _PathJustPassed;

        const int crosswalksCount = 8;
        const int roadsCount = 4;
        const int cameraPollingInterval = 100;
        const int intersectionUpdateInterval = 1000;


        private bool _HasTrafficCollision;
        public bool HasTrafficCollision
        {
            get
            {
                lock (SyncRoot)
                    return _HasTrafficCollision;
            }
            protected set
            {
                bool hasChanged = false;
                lock (SyncRoot)
                {
                    hasChanged = _HasTrafficCollision != value;
                    _HasTrafficCollision = value;
                }
                if (hasChanged)
                    InvokePropertyChanged();
            }
        }

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
                _Crosswalks[i] = new Road(5000, 10000);
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

            //Настраиваем симуляцию прохода через перекрёсток
            
            //Создаём пути через перекрёсток
            _Paths = new IntersectionPath[crosswalksCount + roadsCount];
            for (int i = 0; i < crosswalksCount; i++)
                _Paths[i] = new IntersectionPath(_Crosswalks[i], _TrafficLights[i]);

            for (int i = 0; i < roadsCount; i++)
                _Paths[crosswalksCount + i] = new IntersectionPath(_Roads[i], _TrafficLights[crosswalksCount + i]);

            //Настраиваем пересечения между путями
            //Переход 0 пересекается с дорогами 8-10
            _Paths[0].SetIntersectingPaths(_Paths[8],_Paths[9],_Paths[10]);
            //Переход 1 пересекается с дорогами 9-11
            _Paths[1].SetIntersectingPaths(_Paths[9],_Paths[10], _Paths[11]);
            //Переход 2 пересекается с дорогами 9-11
            _Paths[2].SetIntersectingPaths(_Paths[9],_Paths[10], _Paths[11]);
            //Переход 3 пересекается с дорогами 8, 10, 11
            _Paths[3].SetIntersectingPaths(_Paths[8],_Paths[10],_Paths[11]);
            //Переход 4 пересекается с дорогами 8, 10, 11
            _Paths[4].SetIntersectingPaths(_Paths[8],_Paths[10],_Paths[11]);
            //Переход 5 пересекается с дорогами 8, 9, 11
            _Paths[5].SetIntersectingPaths(_Paths[8],_Paths[9],_Paths[11]);
            //Переход 6 пересекается с дорогами 8, 9, 11
            _Paths[6].SetIntersectingPaths(_Paths[8],_Paths[9],_Paths[11]);
            //Переход 7 пересекается с дорогами 8-10
            _Paths[7].SetIntersectingPaths(_Paths[8],_Paths[9],_Paths[10]);
            //Дорога 8 пересекается с переходами 0, 7, 3-6, дорогами 9, 11
            _Paths[8].SetIntersectingPaths(_Paths[0],_Paths[7],_Paths[3],_Paths[4],_Paths[5],_Paths[6],_Paths[9],_Paths[11]);
            //Дорога 9 пересекается с переходами 0, 7, 1, 2, 5, 6, дорогами 8, 10
            _Paths[9].SetIntersectingPaths(_Paths[0],_Paths[7],_Paths[1],_Paths[2],_Paths[5],_Paths[6],_Paths[8],_Paths[10]);
            //Дорога 10 пересекается с переходами 0, 7, 1-4, дорогами 9, 11
            _Paths[10].SetIntersectingPaths(_Paths[0],_Paths[7],_Paths[1],_Paths[2],_Paths[3],_Paths[4],_Paths[9],_Paths[11]);
            //Дорога 11 пересекается с переходами 1-6, дорогами 8, 10
            _Paths[11].SetIntersectingPaths(_Paths[1],_Paths[2],_Paths[3],_Paths[4],_Paths[5],_Paths[6],_Paths[8],_Paths[10]);

            //Создадём перекрёсток
            _Intersection = new Intersection(intersectionUpdateInterval, _Paths);

            _PathJustPassed = new ObservableCollection<bool>(Enumerable.Repeat(false, _Paths.Length));

            _Simulation.AddEntity(_Intersection);

			_Intersection.OnIntersectionCollision += Intersection_OnIntersectionCollision;
			_Intersection.PathPassed += Intersection_PathPassed;

            _Simulation.StartSimulation();
        }

		private async void Intersection_PathPassed(Intersection intersection, IIntersectionPath path)
		{
            int index = Array.IndexOf(_Paths, path);
            if (index == -1)
                return;
            _PathJustPassed[index] = true;
            await Task.Delay(intersectionUpdateInterval/2);
            if (!HasTrafficCollision)
                _PathJustPassed[index] = false;
		}

		private void Intersection_OnIntersectionCollision(Intersection intersection, IIntersectionPath path)
		{
            //Не ожидаем завершение, потому что завершение совершается изнутри симуляции
			_Simulation.StopSimulation();
            HasTrafficCollision = true;
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
                new int[] { 1, 2, 8 },
                new int[] { 0, 3, 4, 5, 6, 7, 9, 10, 11 }
            );
            yield return state;
         
            //(9 - дорога, 3 и 4 - переходы)
            state = new TrafficLightsState(
                new int[] { 3, 4, 9 },
                new int[] { 0, 1, 2, 5, 6, 7, 8, 10, 11 }
            );
            yield return state;

            //(10 - дорога, 5 и 6 - переходы)
            state = new TrafficLightsState(
                new int[] { 5, 6, 10 },
                new int[] { 0, 1, 2, 3, 4, 7, 8, 9, 11 }
            );
            yield return state;

            //(11 - дорога, 7 и 0 - переходы)
            state = new TrafficLightsState(
                new int[] { 0, 7, 11 },
                new int[] { 1, 2, 3, 4, 5, 6, 8, 9, 10 }
            );
            yield return state;

            yield break;
        }

		public void Dispose()
		{
            if (_Simulation.IsSimulationRunning)
			    _Simulation.StopSimulation().Wait();
		}
	}
}
