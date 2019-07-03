using System;
using System.Net;
using System.Threading.Tasks;
using Unosquare.RaspberryIO.Peripherals;
using Unosquare.RaspberryIO;
using Unosquare.WiringPi;
using Unosquare.RaspberryIO.Abstractions;
using System.Net.Http;

namespace SecuritySystem
{
    class Program
    {
        static void Main(string[] args)
        {
            var ledPin = 27;
            var triggerPin = 17;
            var echoPin = 4;
            var objectDetected = false;

            Pi.Init<BootstrapWiringPi>();

            Pi.Gpio[ledPin].PinMode = GpioPinDriveMode.Output;

            using (var sensor = new UltrasonicHcsr04(Pi.Gpio[triggerPin], Pi.Gpio[echoPin]))
            {
                sensor.OnDataAvailable += async (s, e) =>
                {
                    if (e.IsValid)
                    {
                        if (e.Distance < 50)
                        {
                            Console.WriteLine($"Object detected at {e.Distance:N2}cm!");

                            if (!objectDetected)
                            {
                                objectDetected = true;
                                Pi.Gpio[ledPin].Value = true;
                            }
                        }
                        else
                        {
                            if (objectDetected)
                            {
                                objectDetected = false;
                                Pi.Gpio[ledPin].Value = false;
                            }
                        }
                    }
                };

                sensor.Start();
                Console.ReadLine();
            }
        }
    }
}

