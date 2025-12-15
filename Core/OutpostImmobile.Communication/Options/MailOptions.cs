using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;

namespace OutpostImmobile.Communication.Options;

public class MailOptions
{
    [Required]
    public required string Sender { get; init; }
    [Required]
    public required string SenderMailAddress { get; init; }
    [Required]
    public required string SenderPassword { get; init; }
    [Required]
    public required string SmtpHost { get; init; }
    [Required]
    public required int Port { get ; init; }
}