using System;
using Microsoft.Extensions.Options;

namespace Communication.Options;

public class MailOptions
{
    public string Sender {  get; set; } = string.Empty; 
    public string SenderMailAddress {  get; set; } = string.Empty;
    public string SenderPassword { get; set; } =  string.Empty;
    public string SmtpHost {  get; set; } =  string.Empty;
    public int port {get ; set; }
}