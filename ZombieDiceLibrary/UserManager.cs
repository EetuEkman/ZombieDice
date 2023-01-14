using ZombieDiceLibrary.Models;

namespace ZombieDiceLibrary
{
    public class UserManager
    {
        public List<User> Users { get; private set; } = new();

        public event Action OnChange;

        private void Notify() => OnChange?.Invoke();

        public UserManager()
        {
            Users = new();
        }

        public string AddUser(string userName)
        {
            var user = NewUser(userName);

            Users.Add(user);

            Notify();

            return user.Id;
        }

        public void RemoveUser(string id)
        {
            var user = Users.Where(user => user.Id == id).FirstOrDefault();

            if (user != null)
            {
                Users.Remove(user);

                Notify();
            }
        }

        public void UpdateUser(string id, User user)
        {
            var index = Users.FindIndex(user => user.Id == id);

            if (index != -1)
            {
                Users[index] = user;

                Notify();
            }
        }

        public User NewUser(string userName)
        {
            var user = new User
            {
                Id = Utilities.GetRandomString(),
                Name = userName
            };

            return user;
        }
    }
}
