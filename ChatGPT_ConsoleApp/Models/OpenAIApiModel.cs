namespace ChatGPT_ConsoleApp.Models
{
    public class OpenAIApiModel
    {
        public OpenAIApiModel()
        {
            this.Model = "gpt-3.5-turbo";
            this.Temperature = 0;

            this.Messages = new List<Message>();
        }

        public string Model { get; set; }

        public IList<Message> Messages { get; set; }

        public int Temperature { get; set; }

        public OpenAIApiModel AddMessage(string Message, string Role)
        {
            Message message = new()
            {
                Role = Role,
                Content = Message
            };

            this.Messages.Add(message);

            return this;

        }
    }
}
