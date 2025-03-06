﻿using PrimalEditor.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace PrimalEditor.GameProject
{
    [DataContract(Name = "Game")]
    public class Project : ViewModelBase
    {

        public static string Extension { get; } = ".primal";

        [DataMember]
        public string Name
        {
            get; private set;
        } = "NewProject";

        [DataMember]
        public string Path
        {
            get; private set;
        }

        public string FullPath => $"{Path}{Name}{Extension}";

        [DataMember (Name = "Scenes")]
        private ObservableCollection<Scene> _scenes = new ObservableCollection<Scene>();
        public ReadOnlyObservableCollection<Scene> Scenes
        {
            get; private set;
        }

        private Scene _activeScene;

        public Scene ActiveScene
        {
            get => _activeScene;
            set
            {
                if (value != _activeScene)
                    _activeScene = value;
                OnPropertyChanged(nameof(_activeScene));
            }
        }
        public static Project Current => Application.Current.MainWindow.DataContext as Project;

        public static UndoRedo UndoRedo { get; } = new UndoRedo();

        public ICommand AddScene { get; private set; }
        public ICommand RemoveScene { get; private set; }
        public ICommand Undo { get; private set; }
        public ICommand Redo { get; private set; }
        private void AddSceneInternal(string sceneName)
        {
            Debug.Assert(!string.IsNullOrEmpty(sceneName.Trim()));
            _scenes.Add(new Scene(this, sceneName));
        }

        private void RemoveSceneInternal(Scene scene)
        {
            Debug.Assert(_scenes.Contains(scene));
            _scenes.Remove(scene);
        }
        public void Unload()
        {

        }

        public static Project Load(string file)
        {
            Debug.Assert(File.Exists(file));
            return Serializer.FromFile<Project>(file);
        }

        public static void Save(Project project)
        {
            Serializer.ToFile(project, project.FullPath);
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if ( _scenes != null )
            {
                Scenes = new ReadOnlyObservableCollection<Scene>(_scenes);
                OnPropertyChanged(nameof(Scenes));
            }
            ActiveScene = Scenes.FirstOrDefault(x => x.IsActive);

            AddScene = new RelayCommands<object>(x =>
            {
                AddSceneInternal($"new scene {_scenes.Count}");
                var newScene = _scenes.Last();
                var sceneIndex = _scenes.Count - 1;
                UndoRedo.Add(new UndoRedoAction(
                        () => RemoveSceneInternal(newScene),
                        () => _scenes.Insert(sceneIndex, newScene),
                        $"Add {newScene.Name}")
                    );
            });

            RemoveScene = new RelayCommands<Scene>(x =>
            {
                var sceneIndex = _scenes.IndexOf(x);
                RemoveSceneInternal(x);

                UndoRedo.Add(new UndoRedoAction(
                    () => _scenes.Insert(sceneIndex, x),
                    () => RemoveSceneInternal(x),
                    $"Remove {x.Name}")
                    );
            },x => !x.IsActive
            );

            Undo = new RelayCommands<object>(x => UndoRedo.Undo());
            Redo = new RelayCommands<object>(x => UndoRedo.Redo());
        }
        public Project(string name, string path)
        {
            Name = name;
            Path = path;

            OnDeserialized(new StreamingContext());
        }
    }
}
