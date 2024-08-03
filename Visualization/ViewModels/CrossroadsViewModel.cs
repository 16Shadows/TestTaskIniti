using Simulation.Core;
using Simulation.Roads;
using System;
using System.Collections.Generic;

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

        public CrossroadsViewModel(IContext context) : base(context)
        {
            _Simulation = new SimulationOrchestrator();

            const int crosswalksCount = 8;
            _Crosswalks = new Road[crosswalksCount];
            _CrosswalkVMs = new RoadViewModel[crosswalksCount];
            for (int i = 0; i < crosswalksCount; i++)
            {
                _Crosswalks[i] = new Road(1000, 5000);
                _Simulation.AddEntity(_Crosswalks[i]);
                _CrosswalkVMs[i] = new RoadViewModel(context, _Crosswalks[i]);
            }

            const int roadsCount = 4;
            _Roads = new Road[roadsCount];
            _RoadsVMs = new RoadViewModel[roadsCount];
            for (int i = 0; i < roadsCount; i++)
            {
                _Roads[i] = new Road(2000, 3000);
                _Simulation.AddEntity(_Roads[i]);
                _RoadsVMs[i] = new RoadViewModel(context, _Roads[i]);
            }

            _Simulation.StartSimulation();
        }

		public void Dispose()
		{
			_Simulation.StopSimulation().Wait();
		}
	}
}
