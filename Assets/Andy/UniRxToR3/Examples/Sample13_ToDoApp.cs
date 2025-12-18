// for uGUI(from 4.6)

#if !(UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5)

using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using R3;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using Cysharp.Threading.Tasks;
using ObservableCollections;
using System.Collections;
using System.Threading.Tasks;
using R3.Triggers;
using UnityEngine.EventSystems;

namespace UniRx.Examples
{
    public class Sample13_ToDoApp : MonoBehaviour
    {
        // Open Sample13Scene. Set from canvas
        public Text Title;
        public InputField ToDoInput;
        public Button AddButton;
        public Button ClearButton;
        public GameObject TodoList;

        // prefab:)
        public GameObject SampleItemPrefab;

        ObservableCollection<GameObject> toDos = new();

        void Start()
        {
            // merge Button click and push enter key on input field.
            var submit = Observable.Merge(
                AddButton.OnClickAsObservable().Select(_ => ToDoInput.text),
                ToDoInput.OnEndEditAsObservable().Where(_ => Input.GetKeyDown(KeyCode.Return)));

            // add to reactive collection
            submit.Where(x => x != "")
                .Subscribe(x =>
                {
                    ToDoInput.text = ""; // clear input field
                    var item = Instantiate(SampleItemPrefab) as GameObject;
                    ((Text)item.GetComponentInChildren(typeof(Text))).text = x;
                    toDos.Add(item);
                });

            toDos.CollectionChanged += async (sender, e) => { await OnChanged(e); };
            // Collection Change Handling
            // toDos.ObserveCountChanged().Subscribe(x => Title.text = "TODO App, ItemCount:" + x);
            // toDos.ObserveAdd().Subscribe(x => { x.Value.transform.SetParent(TodoList.transform, false); });
            // toDos.ObserveRemove().Subscribe(x => { GameObject.Destroy(x.Value); });

            // Clear
            ClearButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    var removeTargets = toDos.Where(x => x.GetComponent<Toggle>().isOn).ToArray();
                    foreach (var item in removeTargets)
                    {
                        toDos.Remove(item);
                    }
                });
        }

        async Task OnChanged(NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (GameObject newItem in e.NewItems)
                    {
                        Debug.Log("Added: " + newItem);
                    }

                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (GameObject oldItem in e.OldItems)
                    {
                        Debug.Log("Remove: " + oldItem);
                    }
                    break;

                case NotifyCollectionChangedAction.Move:
                    foreach (GameObject newItem in e.NewItems)
                    {
                        Debug.Log("Move: " + newItem);
                    }

                    foreach (GameObject oldItem in e.OldItems)
                    {
                        Debug.Log("Move: " + oldItem);
                    }

                    break;
                case NotifyCollectionChangedAction.Replace:

                    foreach (GameObject newItem in e.NewItems)
                    {
                        Debug.Log("Replace: " + newItem);
                    }

                    foreach (GameObject oldItem in e.OldItems)
                    {
                        Debug.Log("Replace: " + oldItem);
                    }

                    break;
                case NotifyCollectionChangedAction.Reset:
                    Debug.Log("Reset");

                    break;
            }
        }
    }
}

#endif