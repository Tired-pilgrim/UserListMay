using System.Runtime.Serialization;

namespace ModelLib
{
    public class User
    {
        private string? name;
        [DataMember]
        public string? Name
        {
            get => name;
            set
            {
                if (value != name)
                {
                    name = value;
                    //OnPropertyChanged();
                }
            }
        }

        private string? family;
        [DataMember]
        public string? Family
        {
            get => family;
            set
            {
                if (value != family)
                {
                    family = value;
                    //OnPropertyChanged();
                }
            }
        }
        private string? job;
        [DataMember]
        public string? Job
        {
            get => job;
            set
            {
                if (value != job)
                {
                    job = value;
                    //OnPropertyChanged();
                }
            }
        }
    }
}
