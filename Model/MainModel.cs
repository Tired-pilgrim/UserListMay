using ModelLib;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;

namespace Model
{
    public class MainModel
    {
        public ReadOnlyObservableCollection<User> Users { get; }
        public ObservableCollection<User> privateUsers { get;  } = new()
        {
            new User { Name = "Вася", Family = "Васильев", Job="Студент" },
            new User { Name = "Николай", Family = "Алексеев", Job="Аспирант"},
            new User { Name = "Сидор", Family = "Сидоров", Job="Ректор" }
        };
        public MainModel()
        {
            Users = new(privateUsers);
        }
        public event EventHandler<string>? Message;
        public void AddUzer(User user)
        {
            lock (((ICollection)privateUsers).SyncRoot)
                privateUsers.Add(user);
        }
        public void RemoveUzer(User user)
        {
            lock (((ICollection)privateUsers).SyncRoot)
                privateUsers.Remove(user);
        }
        public void ClearUzer()
        {
            lock (((ICollection)privateUsers).SyncRoot)
                privateUsers.Clear();
        }
        public void OpenList(string path)
        {            
            if (!string.IsNullOrEmpty(path))
            {
                string StrJson = File.ReadAllText(path);
                if (!string.IsNullOrEmpty(StrJson))
                {
                    lock (((ICollection)privateUsers).SyncRoot)
                    {
                        try
                        {
                            User[]? users = JsonSerializer.Deserialize<User[]>(StrJson);
                            privateUsers.Clear();
                            if (users != null)
                            {
                                foreach (User user in users)
                                {
                                    privateUsers.Add(user);
                                }
                                Message?.Invoke(this, "Открыт новый список");
                            }
                        }
                        catch
                        {
                            Message?.Invoke(this, "Не удалось открыть список");
                        }
                    }
                }
            }
            
        }
        public void SaveList(string path)
        {
            if (privateUsers.Count > 0) 
            {
                lock (((ICollection)privateUsers).SyncRoot)
                {
                    string StrJson = JsonSerializer.Serialize(privateUsers);
                    File.WriteAllText(path, StrJson);
                }
            }
        }
     }
}
