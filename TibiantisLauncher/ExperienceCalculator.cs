using System;
using System.Diagnostics;

namespace TibiantisLauncher
{
    internal struct ExperienceStats
    {
        public int? Level { get; set; } = null;
        public int? Experience { get; set; } = null;
        public int? ExperienceForLevel { get; set; } = null;
        public int? ExperienceForNextLevel { get; set; } = null;
        public int? RemainingExperience { get; set; } = null;
        public int? ExperiencePerHour { get; set; } = null;
        public int? RemainingTotalMinutes { get; set; } = null;
        public DateTime? EstimatedAdvanceTime { get; set; } = null;

        public ExperienceStats() { }
    }

    internal class ExperienceCalculator
    {
        private DateTime? _sessionResetSchedule;
        private Stopwatch _sessionStopWatch = new();
        private int _sessionInitialExperience;
        private ExperienceStats _experienceStats;

        public ExperienceStats ExperienceStats => _experienceStats;

        public ExperienceCalculator(int? experience)
        {
            Reset(experience);
        }

        private void Reset(int? experience)
        {
            _experienceStats = new ExperienceStats { Experience = experience };

            if (experience != null)
                _experienceStats.Level = GetLevelFromExperience(experience.Value);

            _sessionInitialExperience = experience ?? 0;
            _sessionResetSchedule = null;
            _sessionStopWatch.Stop();
        }

        public void Tick(int? experience)
        {
            if (experience == null || experience.Value < 0)
                return;

            var experienceGain = experience.Value - _experienceStats.Experience ?? 0;

            if (_sessionResetSchedule < DateTime.Now || experienceGain < 0)
            {
                Reset(experience.Value);
                return;
            }


            _experienceStats.Experience = experience;
            _experienceStats.Level = GetLevelFromExperience(experience.Value);

            if (experienceGain > 0)
            {
                if (!_sessionStopWatch.IsRunning)
                    _sessionStopWatch.Restart();

                _sessionResetSchedule = DateTime.Now.AddMinutes(10);
            }

            if (_experienceStats.Level != null)
            {
                _experienceStats.ExperienceForLevel = GetExperienceForLevel(_experienceStats.Level.Value);
                _experienceStats.ExperienceForNextLevel = GetExperienceForLevel(_experienceStats.Level.Value + 1);
                _experienceStats.RemainingExperience = _experienceStats.ExperienceForNextLevel - experience;
            }

            if (_experienceStats.RemainingExperience != null)
            {
                if (_sessionStopWatch.IsRunning || _sessionStopWatch.Elapsed.TotalSeconds > 0)
                {
                    var totalExperienceGain = experience.Value - _sessionInitialExperience;
                    _experienceStats.ExperiencePerHour = RoundToHundred((int)Math.Floor(totalExperienceGain / (double)Math.Max(_sessionStopWatch.Elapsed.TotalHours, 0.01)));

                    var remainingMinutes = _experienceStats.ExperiencePerHour > 0 ? _experienceStats.RemainingExperience / ((double)_experienceStats.ExperiencePerHour / 60) : 0;
                    _experienceStats.RemainingTotalMinutes = (int)Math.Round((decimal)remainingMinutes);
                    _experienceStats.EstimatedAdvanceTime = DateTime.Now.AddMinutes((double)_experienceStats.RemainingTotalMinutes);
                }
            }
        }

        private int RoundToHundred(int number)
        {
            return (number + 50) / 100 * 100;
        }

        public static int GetExperienceForLevel(int level)
        {
            var a = 50 * Math.Pow(level, 3);
            var b = 300 * Math.Pow(level, 2);
            var c = 850 * level;

            var experience = Math.Floor((a - b + c - 600) / 3);

            return (int)experience;
        }

        public static int GetLevelFromExperience(int experience)
        {
            for (int level = 1; level < 1000; level++)
            {
                if (GetExperienceForLevel(level + 1) > experience)
                    return level;
            }

            return 1;
        }
    }
}
