using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AGIle_Robotics.Updater
{
    public class BaseViewModel
    {
        protected Dictionary<string, object> Locks { get => locks; set => locks = value; }
        private Dictionary<string, object> locks = new Dictionary<string, object>();
        protected void AddLock(string name)
        {
            lock (Locks)
            {
                Locks.Add(name, new object());
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            OnPropertyChanged(propertyName);
        }
        
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual bool SetProperty<T>(ref T target, T value, [CallerMemberName] string propertyName = null)
        {
            lock (Locks)
            {
                if (!Locks.ContainsKey(propertyName))
                {
                    AddLock(propertyName);
                }
            }

            lock (Locks[propertyName]) // TryGetValue just repeats what ContainsKey did
            {
                if (Equals(target, value)) return false;

                target = value;
                OnPropertyChanged(propertyName);
                return true;
            }
        }

        protected virtual T GetProperty<T>(ref T target, [CallerMemberName] string propertyName = null)
        {
            lock (Locks)
            {
                if (!Locks.ContainsKey(propertyName))
                {
                    AddLock(propertyName);
                }
            }

            lock (Locks[propertyName])
            {
                return target;
            }
        }
    }
}
