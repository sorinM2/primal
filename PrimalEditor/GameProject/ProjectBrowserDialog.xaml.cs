using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PrimalEditor.GameProject
{
    /// <summary>
    /// Interaction logic for ProjectBrowserDialg.xaml
    /// </summary>
    public partial class ProjectBrowserDialog : Window
    {
        private CubicEase _easing = new CubicEase() { EasingMode = EasingMode.EaseInOut };
        public ProjectBrowserDialog()
        {
            InitializeComponent();
            Loaded += OnProjectBrowserOpened;
        }

        private void OnProjectBrowserOpened(object sender, RoutedEventArgs e)
        {
            Loaded -= OnProjectBrowserOpened;
            if (!OpenProject.Projects.Any())
            {
                openProjectButton.IsEnabled = false;
                openProjectView.Visibility = Visibility.Hidden;
                OnToggleButton_Click(createProjectButton, new RoutedEventArgs());
            }
        }

        private void AnimateToCreateProject()
        {
            var highlightAnimation = new DoubleAnimation(225, 425, new Duration(TimeSpan.FromSeconds(0.2)));
            highlightAnimation.EasingFunction = _easing;
            highlightAnimation.Completed += (s, e) =>
            {
                var animation = new ThicknessAnimation( new Thickness(0), new Thickness(-1600, 0, 0, 0), new Duration(TimeSpan.FromSeconds(0.5)));
                animation.EasingFunction = _easing;
                browserContent.BeginAnimation(MarginProperty, animation);
            };
            highlightRect.BeginAnimation(Canvas.LeftProperty, highlightAnimation);
        }
        private void AnimateToOpenProject()
        {
            var highlightAnimation = new DoubleAnimation(425, 225, new Duration(TimeSpan.FromSeconds(0.2)));
            highlightAnimation.EasingFunction = _easing;
            highlightAnimation.Completed += (s, e) =>
            {
                var animation = new ThicknessAnimation(new Thickness(-1600, 0, 0, 0), new Thickness(0), new Duration(TimeSpan.FromSeconds(0.5)));
                animation.EasingFunction = _easing;
                browserContent.BeginAnimation(MarginProperty, animation);
            };
            highlightRect.BeginAnimation(Canvas.LeftProperty, highlightAnimation);
        }
        private void OnToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if ( sender == openProjectButton )
            {
                if ( createProjectButton.IsChecked == true )
                {
                    createProjectButton.IsChecked = false;
                    AnimateToOpenProject();
                    newProjectView.IsEnabled = false;
                    openProjectView.IsEnabled = true;
                }
                openProjectButton.IsChecked = true;
            }
            else
            {
                if ( openProjectButton.IsChecked == true )
                {
                    openProjectButton.IsChecked = false;
                    AnimateToCreateProject();
                    newProjectView.IsEnabled = true;
                    openProjectView.IsEnabled = false;
                }
                createProjectButton.IsChecked = true;
            }
        }
    }
}
