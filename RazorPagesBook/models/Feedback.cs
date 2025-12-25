using System.ComponentModel.DataAnnotations;


namespace RazorPagesBook.Models
{
    public class Feedback
    {
        public int Id { get; set; }


        [Required, StringLength(80)]
        public string Name { get; set; } = string.Empty;


        [Required, EmailAddress, StringLength(120)]
        public string Email { get; set; } = string.Empty;


        [Required, StringLength(120)]
        public string Subject { get; set; } = string.Empty;


        [Required, StringLength(4000)]
        public string Message { get; set; } = string.Empty;


        [StringLength(450)]
        public string? UserId { get; set; }    // якщо користувач залогінений


        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [StringLength(200)]
        public string? AdminReplySubject { get; set; }


        [StringLength(4000)]
        public string? AdminReplyBody { get; set; }


        [StringLength(128)]
        public string? AdminRepliedBy { get; set; }


        public DateTime? AdminRepliedAt { get; set; }
    }
}

