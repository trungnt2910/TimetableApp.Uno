using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TimetableApp.Core;
#if !HAS_UNO
using Windows.System.Threading;
#else
using TimetableApp.Hacks.System.Threading;
#endif
using Windows.UI.Core;
using Windows.UI.Xaml.Data;

namespace TimetableApp.ViewModels
{
    [Windows.UI.Xaml.Data.Bindable]
    public class HomePageViewModel : BindableBase
    {
        private const string NoClasses = "**HOORAY! NO CLASSES!**";
        private ThreadPoolTimer periodicTimer; 
        private Task periodicTask;
        private CancellationTokenSource periodicTaskCancellationTokenSource = new CancellationTokenSource();
        private DateTime lastCheck = DateTime.Now;
        private Lesson currentLesson;
        private Lesson nextLesson;

#region Settings
        private bool doAutoJoin = false;
        private string displayName;
        public string DisplayName
        {
            get => displayName;
            set
            {
                SetProperty(ref displayName, value);
            }
        }
        private TimeSpan? reportBeforeTime;
        private TimeSpan? allowJoinBeforeTime;
#endregion
#region Properites
        private string displayText = "Your next lesson";
        public string DisplayText
        {
            get => displayText;
            set => SetProperty(ref displayText, value);
        }
        private string lessonInfoText = NoClasses;
        public string LessonInfoText
        {
            get => lessonInfoText;
            set => SetProperty(ref lessonInfoText, value);
        }
        private bool joinButtonIsEnabled = false;
        public bool JoinButtonIsEnabled
        {
            get => joinButtonIsEnabled;
            set => SetProperty(ref joinButtonIsEnabled, value);
        }
        private ObservableCollection<Lesson> todayLessons = new ObservableCollection<Lesson>();
        public ObservableCollection<Lesson> TodayLessons
        {
            get => todayLessons;
            set => SetProperty(ref todayLessons, value);
        }
        private CollectionViewSource weekLessons = new CollectionViewSource();
        public CollectionViewSource WeekLessons
        {
            get => weekLessons;
            set => SetProperty(ref weekLessons, value);
        }
#endregion

        public HomePageViewModel()
        {
            periodicTimer = ThreadPoolTimer.CreatePeriodicTimer((source) => Periodic(), TimeSpan.FromSeconds(1));
            Data.Timetable.OnSucessfulUpdate += (sender, args) => { RunOnMainThreadAsync(ReloadTabs); };
            RunOnMainThreadAsync(ReloadToday);
            RunOnMainThreadAsync(ReloadThisWeek);
        }

        private async Task RunOnMainThreadAsync(Action a)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => a());
        }

        private async Task Periodic()
        {
            var currentCheck = DateTime.Now;
            await RunOnMainThreadAsync(() => System.Diagnostics.Debug.WriteLine("Checking..."));

            if (periodicTaskCancellationTokenSource.Token.IsCancellationRequested) return;

            SetCurrentLesson(Data.Timetable.GetCurrentLesson());

            if (currentLesson == null)
            {   
                nextLesson = Data.Timetable.GetNextLesson(reportBeforeTime);
                await RunOnMainThreadAsync(() => DisplayText = "Your next lesson");
            }
            else
            {
                await RunOnMainThreadAsync(() => DisplayText = "Your current lesson");
            }

            await RunOnMainThreadAsync(() =>
            {
                LessonInfoText = currentLesson?.ToMarkdown() ?? (nextLesson?.ToMarkdown() ?? NoClasses);
                JoinButtonIsEnabled = (currentLesson != null) || (nextLesson != null && Data.Timetable.CheckNextLesson(allowJoinBeforeTime));
            });

            if (lastCheck.DayOfYear != currentCheck.DayOfYear)
            {
                await RunOnMainThreadAsync(ReloadToday);
            }
            lastCheck = currentCheck;
        }

        private void SetCurrentLesson(Lesson newLesson)
        {
            if (currentLesson != newLesson)
            {
                currentLesson = newLesson;
                if (doAutoJoin) AutoJoin();
            }
        }

        private void AutoJoin()
        {
            if (currentLesson == null) return;
            if (currentLesson.Credentials.SupportsOneClick)
            {
                currentLesson.Credentials.EnterClass(new StudentInfo() { Name = DisplayName });
            }
        }

        public void ReloadToday()
        {
            var day = (int)DateTime.Now.DayOfWeek;
            todayLessons.Clear();
            foreach (var l in Data.Timetable.Lessons[day])
            {
                todayLessons.Add(l);
            }
        }

        public void ReloadThisWeek()
        {
            ObservableCollection<ObservableCollection<Lesson>> groups = new ObservableCollection<ObservableCollection<Lesson>>();
            for (int i = 0; i < Data.Timetable.Lessons.Length; ++i)
            {
                ObservableCollection<Lesson> info = new ObservableCollection<Lesson>();
                foreach (var lesson in Data.Timetable.Lessons[i])
                {
                    lesson.DayOfWeek = ((DayOfWeek)(i)).ToString();
                    info.Add(lesson);
                }
                if (info.Count == 0) continue;
                groups.Add(info);
            }

            WeekLessons = new CollectionViewSource()
            {
                IsSourceGrouped = true,
                Source = groups
            };
        }

        private void ReloadTabs()
        {
            ReloadToday();
            ReloadThisWeek();
        }

        public void TryEnterClass(StudentInfo info)
        {
            if (currentLesson == null)
            {
                nextLesson.EnterClass(info);
            }
            else
            {
                currentLesson.EnterClass(info);
            }
        }
    }
}
