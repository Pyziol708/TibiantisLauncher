using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TibiantisLauncher.Clients;

namespace TibiantisLauncher
{
    internal struct ExperienceStats
    {
        public int ExperienceForLevel { get; set; }
        public int ExperienceForNextLevel { get; set; }
        public int RemainingExperience { get; set; }
        public int ExperiencePerHour { get; set; }
        public int RemainingTotalMinutes { get; set; }
    }

    internal class ExperienceCalculator
    {
        private DateTime? _sessionResetSchedule;
        private Stopwatch _sessionStopWatch = new Stopwatch();
        private int _experience;
        private int _experienceGained;
        private ExperienceStats _experienceStats;

        public ExperienceStats ExperienceStats => _experienceStats;

        public ExperienceCalculator(int experience)
        {
            Reset(experience);
        }

        public void Reset(int experience)
        {
            _experience = experience;
            _experienceGained = 0;
            _sessionResetSchedule = null;
            _sessionStopWatch.Stop();
            _experienceStats = new ExperienceStats
            {
                ExperienceForLevel = 0,
                ExperienceForNextLevel = 0,
                ExperiencePerHour = 0,
                RemainingExperience = 0,
                RemainingTotalMinutes = 0
            };
        }

        public void Tick(int level, int experience)
        {
            var experienceGain = experience - _experience;

            if (_sessionResetSchedule < DateTime.Now)
            {
                Reset(experience);
                return;
            }

            _experience = experience;

            if (experienceGain > 0)
            {
                if (!_sessionStopWatch.IsRunning)
                    _sessionStopWatch.Restart();

                _experienceGained += experienceGain;
                _sessionResetSchedule = DateTime.Now.AddMinutes(15);
            }

            _experienceStats.ExperienceForLevel = GetExperienceForLevel(level);
            _experienceStats.ExperienceForNextLevel = GetExperienceForLevel(level + 1);
            _experienceStats.RemainingExperience = _experienceStats.ExperienceForNextLevel - experience;

            if (_sessionStopWatch.IsRunning || _sessionStopWatch.Elapsed.TotalSeconds > 0)
            {
                _experienceStats.ExperiencePerHour = RoundToHundred((int)Math.Floor(_experienceGained * 3600 / _sessionStopWatch.Elapsed.TotalSeconds));
                var remainingMinutes = _experienceStats.ExperiencePerHour > 0 ? _experienceStats.RemainingExperience / (_experienceStats.ExperiencePerHour / 60) : 0;
                _experienceStats.RemainingTotalMinutes = remainingMinutes;
            }
        }

        private int RoundToHundred(int number)
        {
            return (number + 50) / 100 * 100;
        }

        private int GetExperienceForLevel(int level)
        {
            var a = 50 * Math.Pow(level, 3);
            var b = 300 * Math.Pow(level, 2);
            var c = 850 * level;
            var exp = Math.Floor((a - b + c - 600) / 3);
            
            return (int)exp;
        }
    }
}
