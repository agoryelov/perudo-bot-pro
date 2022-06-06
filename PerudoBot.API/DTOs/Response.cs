namespace PerudoBot.API.DTOs
{
    public class Response
    {
        public bool RequestSuccess { get; set; } = true;
        public string ErrorMessage { get; set; }
    }

    public static class Responses
    {
        public static Response OK()
        {
            return new Response();
        }

        public static Response Error(string messsage)
        {
            return new Response
            {
                RequestSuccess = false,
                ErrorMessage = messsage
            };
        }
    }
}
