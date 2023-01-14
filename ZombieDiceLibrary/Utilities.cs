namespace ZombieDiceLibrary
{
    class Utilities
    {
        public static string GetRandomString(int length = 8)
        {
            var random = Random.Shared;

            var randomString = "";

            for (int i = 0; i < length; i++)
            {
                var number = random.Next(1, 3) == 1 ? random.Next(65, 90) : random.Next(97, 122);

                var character = Convert.ToChar(number);

                randomString += character;
            }

            return randomString;
        }
    }
    
}
