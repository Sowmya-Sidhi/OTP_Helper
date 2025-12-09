# OTP_Helper

[![NuGet](https://img.shields.io/nuget/v/OTP_Helper.svg)](https://www.nuget.org/packages/OTP_Helper/)
[![License](https://img.shields.io/github/license/Sowmya-Sidhi/OTP_Helper)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-8.0-blue)](https://dotnet.microsoft.com/)

**OTP_Helper** is a .NET library for secure **password reset functionality** using **MongoDB**, **email notifications via SMTP**, and **password hashing**.

---

## 📌 Table of Contents

- [Prerequisites](#-prerequisites)
- [NuGet Packages](#-nuget-packages)
- [Google SMTP Setup](#-google-smtp-setup)
- [Configuration](#-configuration)
- [Adding Library to Your Project](#-adding-library-to-your-project)
- [Usage Example](#-usage-example)
- [Notes](#-notes)
- [License](#-license)

---

## ⚙️ Prerequisites

- .NET 8.0 or higher
- MongoDB (local or remote instance)
- Gmail account (for SMTP email sending)
- GitHub account (if using library as a Git submodule or GitHub Packages)

---

## 📦 NuGet Packages

Install these packages in your project:

```bash
dotnet add package MailKit
dotnet add package Microsoft.Extensions.Logging
dotnet add package Microsoft.Extensions.Logging.Console
dotnet add package BCrypt.Net-Next
dotnet add package MongoDB.Driver

---
## Google SMTP Setup

To send emails using Gmail SMTP:
Go to Google Account
 → Security
Enable 2-Step Verification
Go to App Passwords: Direct Link
Create a new App Password:
App: Mail
Device: Other (give a name, e.g., OTP_Helper)
Copy the 16-digit password (e.g., abcd efgh ijkl mnop) and save it securely.

##Configuration

"Smtp": {
  "Host": "smtp.gmail.com",
  "Port": "587",
  "Username": "<Your Gmail ID>",
  "password": "<Your 16-Digit App Password>",
  "From": "<Your Gmail ID>"
}

##Adding Library to your Project

After publishing to GitHub Packages:
dotnet add package OTP_Helper --version 1.0.0 --source https://nuget.pkg.github.com/Sowmya-Sidhi/index.json

##Usage Example:
using OTP_Helper.Services;
using OTP_Helper.Interfaces;
using OTP_Helper.Settings;
using Microsoft.Extensions.Options;

// Configure SMTP settings
var emailService = new MailKitEmailService(new OptionsWrapper<SmtpSettings>(smtpSettings));

// Initialize the password reset service
var passwordResetService = new PasswordResetService(userRepository, emailService);

// Request password reset OTP
await passwordResetService.RequestPasswordResetAsync("user@example.com");

##📌 Notes

Ensure your MongoDB database and collections exist before requesting OTPs.
Only registered users in your database will receive OTPs.
You can configure OTP length, expiry, and attempts in the library.
Logging is enabled via Microsoft.Extensions.Logging.

##📜 License

This library is open-sourced under the MIT License.





