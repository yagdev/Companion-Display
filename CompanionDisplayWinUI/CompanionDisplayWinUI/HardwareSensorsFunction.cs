using LibreHardwareMonitor.Hardware;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static CompanionDisplayWinUI.WidgetSensors;

namespace CompanionDisplayWinUI
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
        public Computer computer = new Computer
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
            computer.Open();
            computer.Accept(new UpdateVisitor());
            Thread thread = new(UpdateSensor);
            thread.Start();
        }
        public void UpdateSensor() 
        {
            UpdateVisitor update = new UpdateVisitor();
            update.VisitComputer(computer);
            Thread.Sleep(3000);
            Thread thread = new(UpdateSensor);
            thread.Start();
        }
    }
}
