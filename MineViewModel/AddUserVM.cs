using Model;
using ModelLib;
using VievModelLib;
using ViewModelLib.Commands;

namespace ViewModel
{
    public class AddUserVM : ViewModelBase
    {
        public RelayCommand AddUserCommand { get; }
        public AddUserVM(MainModel model)
        {
            AddUserCommand = new RelayCommand(
                () =>
                {
                    User user = new User { Name = this.Name, Family = this.Family, Job = this.Job };
                    model.AddUzer(user);
                    Name = string.Empty; Family = string.Empty; Job = string.Empty; 
                },
                () => !(string.IsNullOrWhiteSpace(Name) ||
                          string.IsNullOrWhiteSpace(Family) ||
                          string.IsNullOrWhiteSpace(Job)));
        }
        private string _name = string.Empty;
        public string Name
        {
            get => _name;
            set => Set(ref _name, ref value);
        }

        private string _family = string.Empty;
        public string Family
        {
            get => _family;
            set => Set(ref _family, ref value);
        }
        private string _job = string.Empty;
        public string Job
        {
            get => _job;
            set => Set(ref _job, ref value);
        }
    }
}
