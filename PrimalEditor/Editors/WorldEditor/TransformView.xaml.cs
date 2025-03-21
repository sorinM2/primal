using PrimalEditor.Components;
using PrimalEditor.GameProject;
using PrimalEditor.Utilities;
using System;
using System.Diagnostics;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;



namespace PrimalEditor.Editors
{
    /// <summary>
    /// Interaction logic for TransformView.xaml
    /// </summary>
    public partial class TransformView : UserControl
    {
        private Action _undoAction = null;
        private bool _propertyChanged = false;
        public TransformView()
        {
            InitializeComponent();
            Loaded += OnTransformViewLoaded;
        }

        private void OnTransformViewLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnTransformViewLoaded;
            (DataContext as MSTransform).PropertyChanged += (s, e) => _propertyChanged = true;
        }

        private Action GetAction(Func<Transform, (Transform transform, Vector3)> selector, 
                                Action<(Transform transform, Vector3)> forEachAction)
        {
            if (!(DataContext is MSTransform vm))
            {
                return null;
            }
            var selection = vm.SelectedComponents.Select(transform => selector(transform)).ToList();
            return new Action(() =>
            {
                selection.ForEach(item => forEachAction(item));
                (GameEntityView.Instance.DataContext as MSEntity)?.GetMSComponent<MSTransform>().Refresh();
            });
        }

        private Action GetPositionAction() => GetAction(transform => (transform, transform.Position), (item) => item.transform.Position = item.Item2);
        private Action GetRotationAction() => GetAction(transform => (transform, transform.Rotation), (item) => item.transform.Rotation = item.Item2);
        private Action GetScaleAction() => GetAction(transform => (transform, transform.Scale), (item) => item.transform.Scale = item.Item2);

        private void RecordAction(Action redoAction, string name)
        {
            if (_propertyChanged)
            {
                Debug.Assert(_undoAction != null);
                _propertyChanged = false;
                Project.UndoRedo.Add(new UndoRedoAction(_undoAction, redoAction, name));
            }
        }
        private void OnPosition_VectorBox_PreviewMouse_LBD(object sender, MouseButtonEventArgs e)
        {
            _propertyChanged = false;
            _undoAction = GetPositionAction();
        }

        private void OnPosition_VectorBox_PreviewMouse_LBU(object sender, MouseButtonEventArgs e)
        {
            RecordAction(GetPositionAction(), "Position changed");
        }
        private void OnRotation_VectorBox_PreviewMouse_LBD(object sender, MouseButtonEventArgs e)
        {
            _propertyChanged = false;
            _undoAction = GetRotationAction();
        }

        private void OnRotation_VectorBox_PreviewMouse_LBU(object sender, MouseButtonEventArgs e)
        {
            RecordAction(GetRotationAction(), "Rotation changed");
        }
        private void OnScale_VectorBox_PreviewMouse_LBD(object sender, MouseButtonEventArgs e)
        {
            _propertyChanged = false;
            _undoAction = GetScaleAction();
        }

        private void OnScale_VectorBox_PreviewMouse_LBU(object sender, MouseButtonEventArgs e)
        {
            RecordAction(GetScaleAction(), "Scale changed");
        }

        private void OnPosition_VectorBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if ( _propertyChanged && _undoAction != null )
            {
                OnPosition_VectorBox_PreviewMouse_LBU(sender, null);
            }
        }
        private void OnRotation_VectorBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if ( _propertyChanged && _undoAction != null )
            {
                OnRotation_VectorBox_PreviewMouse_LBU(sender, null);
            }
        }
        private void OnScale_VectorBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if ( _propertyChanged && _undoAction != null )
            {
                OnScale_VectorBox_PreviewMouse_LBU(sender, null);
            }
        }
    }
}
