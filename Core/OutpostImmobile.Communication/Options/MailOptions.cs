using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;

namespace OutpostImmobile.Communication.Options;

public class MailOptions
{
    [Required]
    public string Sender { get; init; } = string.Empty; 
    [Required]
    public string SenderMailAddress { get; init; } = string.Empty;
    [Required]
    public string SenderPassword { get; init; } = string.Empty;
    [Required]
    public string SmtpHost { get; init; } = string.Empty;
    [Required]
    public int Port { get ; init; }
}