using PrimalEditor.ContentToolsAPIStructs;
using PrimalEditor.DllWrappers;
using PrimalEditor.Editors;
using PrimalEditor.Utilities.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PrimalEditor.Content
{
    /// <summary>
    /// Interaction logic for PrimitiveMeshDialog.xaml
    /// </summary>
    public partial class PrimitiveMeshDialog : Window
    {
        private static readonly List<ImageBrush> _textures = new List<ImageBrush>();
        public PrimitiveMeshDialog()
        {
            InitializeComponent();
            Loaded += (s, e) => UpdatePrimitive();
        }
        private void OnPrimitiveType_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) => UpdatePrimitive();


        private void OnSlider_Value_Changed(object sender, RoutedPropertyChangedEventArgs<double> e) => UpdatePrimitive();
        private void OnScalarBox_ValueChanged(object sender, RoutedEventArgs e) => UpdatePrimitive();


        private float Value(ScalarBox scalarBox, float min)
        {
            float.TryParse(scalarBox.Value, out var result);
            return Math.Max(result, min);
        }

        private void UpdatePrimitive()
        {
            if (!IsInitialized)
                return;
            var primitiveType = (PrimitiveMeshType)primTypeComboBox.SelectedItem;
            var info = new PrimitiveInitInfo() { Type = primitiveType };

            switch(primitiveType)
            {
                case PrimitiveMeshType.Plane:
                    {
                        info.SegmentX = (int)xSliderPlane.Value;
                        info.SegmentZ = (int)zSliderPlane.Value;
                        info.Size.X = Value(widthScalarBoxPlane, 0.001f);
                        info.Size.Z = Value(heightScalarBoxPlane, 0.001f);
                        break;
                    }
                case PrimitiveMeshType.Cube:
                    {
                        return;
                        break;
                    }
                case PrimitiveMeshType.UvSphere:
                    {
                        return;
                        break;
                    }
                case PrimitiveMeshType.IcoSphere:
                    {
                        return;
                        break;
                    }
                case PrimitiveMeshType.Cylinder:
                    {
                        return;
                        break;
                    }
                case PrimitiveMeshType.Capsule:
                    {
                        return;
                        break;
                    }

            }
            var geometry = new Geometry();
            ContentToolsAPI.CreatePrimitiveMesh(geometry, info);
            (DataContext as GeometryEditor).SetAsset(geometry);
            OnTexture_CheckBox_Click(textureCheckBox, null);
        }

        private static void LoadTextures()
        {
            var uris = new List<Uri>
            {
                new Uri("pack://application:,,,/Resources/PrimitiveMeshView/PlaneTexture.png"),
            };
            _textures.Clear();
            foreach(var uri in uris)
            {
                var resource = Application.GetResourceStream(uri);
                using var reader = new BinaryReader(resource.Stream);
                var data = reader.ReadBytes((int)resource.Stream.Length);
                var imageSource = (BitmapSource)new ImageSourceConverter().ConvertFrom(data);
                imageSource.Freeze();
                var brush = new ImageBrush(imageSource);
                brush.Transform = new ScaleTransform(1, -1, 0.5, 0.5);
                brush.ViewportUnits = BrushMappingMode.Absolute;
                brush.Freeze();
                _textures.Add(brush);
            }
        }
        static PrimitiveMeshDialog()
        {
            LoadTextures();
        }

        private void OnTexture_CheckBox_Click(object sender, RoutedEventArgs e)
        {
            Brush brush = Brushes.White;
            if ( (sender as CheckBox).IsChecked == true)
            {
                brush = _textures[(int)primTypeComboBox.SelectedItem];
            }

            var vm = DataContext as GeometryEditor;
            foreach(var mesh in vm.MeshRenderer.Meshes)
            {
                mesh.Diffuse = brush;
            }
        }
    }
}
