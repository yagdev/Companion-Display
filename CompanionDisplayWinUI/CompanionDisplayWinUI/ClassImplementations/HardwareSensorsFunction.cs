using LibreHardwareMonitor.Hardware;
using System.Threading;

namespace CompanionDisplayWinUI.ClassImplementations
{
    internal class HardwareSensorsFunction
    {
        public class UpdateVisitor : IVisitor
        {
            public void VisitComputer(IComputer computer)
            {
                computer.Traverse(this);
            }
            public void VisitHardware(IHardware hardware)
            {
                hardware.Update();
                foreach (IHardware subHardware in hardware.SubHardware) subHardware.Accept(this);
            }
            public void VisitSensor(ISensor sensor) { }
            public void VisitParameter(IParameter parameter) { }
        }
        public Computer computer = new()
        {
            IsCpuEnabled = true,
            IsGpuEnabled = true,
            IsMemoryEnabled = true,
            IsMotherboardEnabled = true,
            IsControllerEnabled = true,
            IsNetworkEnabled = true,
            IsStorageEnabled = true
        };
        public void Init()
        {
            try
            {
                computer.Open();
                computer.Accept(new UpdateVisitor());
                Thread thread = new(UpdateSensor);
                thread.Start();
            }
            catch { }
        }
        public static event CommonlyAccessedInstances.HandleEventsWithNoArgs UpdateSensorValue;
        static void CallSensorUpdate()
        {
            UpdateSensorValue?.Invoke();
        }
        public void UpdateSensor() 
        {
            if(Globals.CurrentHW != null)
            {
                UpdateVisitor update = new();
                update.VisitHardware(Globals.CurrentHW);
            }
            CallSensorUpdate();
            Thread.Sleep(3000);
            Thread thread = new(UpdateSensor);
            thread.Start();
        }
    }
}
