namespace MyDemoApp
{
    public class MyEntity
    {
        private string name;

        public int Id { get; } // Auto property

        public string Name // Non-auto property
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }
    }
}
