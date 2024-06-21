using AlienUI.Models;
using System;
using System.Timers;

namespace DEMO
{
    public class LoginScreenVM : ViewModel
    {
        public string CurrentTime
        {
            get
            {
                var dateNow = DateTime.Now;
                return $"{dateNow.Hour:00}:{dateNow.Minute:00}:{dateNow.Second:00}";
            }
        }

        public string CurrentDate
        {
            get
            {
                var dateNow = DateTime.Now;
                return $"{dateNow.Month}ÔÂ{dateNow.Day:00}ºÅ,{dateNow.DayOfWeek.GetDayOfWeekStr()}";
            }
        }

        public LoginScreenVM()
        {
            Timer timer = new Timer(1000);
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            WindowsEmulator.Instance.Engine.Dispatch(() =>
            {
                RaisePropertyChanged("CurrentData");
                RaisePropertyChanged("CurrentTime");
            });
        }
    }
}
