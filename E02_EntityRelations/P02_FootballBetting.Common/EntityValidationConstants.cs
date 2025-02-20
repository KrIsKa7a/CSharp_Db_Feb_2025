namespace P02_FootballBetting.Common
{
    public static class EntityValidationConstants
    {
        public static class Team
        {
            public const int TeamNameMaxLength = 75;
            public const int TeamLogoUrlMaxLength = 2048;
            public const int TeamInitialsMaxLength = 5;
        }

        public static class Color
        {
            public const int ColorNameMaxLength = 30;
        }

        public static class Town
        {
            public const int TownNameMaxLength = 60;
        }

        public static class Country
        {
            public const int CountryNameMaxLength = 60;
        }

        public static class Player
        {
            public const int PlayerNameMaxLength = 100;
        }

        public static class Position
        {
            public const int PositionNameMaxLength = 30;
        }

        public static class Game
        {
            public const int GameResultMaxLength = 12;
        }

        public static class User
        {
            public const int UserUsernameMaxLength = 50;
            public const int UserNameMaxLength = 100;
            public const int UserPasswordMaxLength = 512;
            public const int UserEmailMaxLength = 320;
        }
    }
}
