namespace SportsGurukul.Application.Features.Authentication.Constants;

public static class EmailTemplates
{
    public static string VerificationEmail(string firstName, string verificationLink, int expiryHours)
    {
        var year = DateTime.UtcNow.Year;
        var safeName = EscapeHtml(firstName);

        var body = $$"""
            <!DOCTYPE html>
            <html lang="en">
            <head>
                <meta charset="UTF-8">
                <meta name="viewport" content="width=device-width, initial-scale=1.0">
                <title>Verify Your Email - Sports Gurukul</title>
            </head>
            <body style="margin:0;padding:0;background-color:#f4f4f4;font-family:Arial,Helvetica,sans-serif;">
                <table width="100%" cellpadding="0" cellspacing="0" style="background-color:#f4f4f4;padding:40px 0;">
                    <tr>
                        <td align="center">
                            <table width="600" cellpadding="0" cellspacing="0" style="background-color:#ffffff;border-radius:8px;overflow:hidden;">
                                <tr>
                                    <td style="background-color:#4CAF50;padding:30px;text-align:center;">
                                        <h1 style="color:#ffffff;margin:0;font-size:24px;">Sports Gurukul</h1>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="padding:40px 30px;">
                                        <h2 style="color:#333333;margin:0 0 20px;">Welcome, {{safeName}}!</h2>
                                        <p style="color:#555555;font-size:16px;line-height:1.6;margin:0 0 20px;">
                                            Thank you for registering with Sports Gurukul. Please verify your email address by clicking the button below.
                                        </p>
                                        <table cellpadding="0" cellspacing="0" style="margin:0 0 20px;">
                                            <tr>
                                                <td style="background-color:#4CAF50;border-radius:5px;">
                                                    <a href="{{verificationLink}}" style="display:inline-block;padding:14px 30px;color:#ffffff;text-decoration:none;font-size:16px;font-weight:bold;">Verify Email Address</a>
                                                </td>
                                            </tr>
                                        </table>
                                        <p style="color:#555555;font-size:14px;line-height:1.6;margin:0 0 10px;">
                                            This link expires in <strong>{{expiryHours}} hour(s)</strong>.
                                        </p>
                                        <p style="color:#999999;font-size:13px;line-height:1.6;margin:0 0 10px;">
                                            If you did not create an account, please ignore this email.
                                        </p>
                                        <p style="color:#999999;font-size:13px;line-height:1.6;margin:0;">
                                            If the button above doesn't work, copy and paste this link into your browser:<br>
                                            <a href="{{verificationLink}}" style="color:#4CAF50;word-break:break-all;">{{verificationLink}}</a>
                                        </p>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="background-color:#f4f4f4;padding:20px 30px;text-align:center;">
                                        <p style="color:#999999;font-size:12px;margin:0;">
                                            &copy; {{year}} Sports Gurukul. All rights reserved.
                                        </p>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </body>
            </html>
            """;

        return body;
    }

    public static string PasswordResetEmail(string firstName, string resetLink, int expiryMinutes)
    {
        var year = DateTime.UtcNow.Year;
        var safeName = EscapeHtml(firstName);

        var body = $$"""
            <!DOCTYPE html>
            <html lang="en">
            <head>
                <meta charset="UTF-8">
                <meta name="viewport" content="width=device-width, initial-scale=1.0">
                <title>Reset Your Password - Sports Gurukul</title>
            </head>
            <body style="margin:0;padding:0;background-color:#f4f4f4;font-family:Arial,Helvetica,sans-serif;">
                <table width="100%" cellpadding="0" cellspacing="0" style="background-color:#f4f4f4;padding:40px 0;">
                    <tr>
                        <td align="center">
                            <table width="600" cellpadding="0" cellspacing="0" style="background-color:#ffffff;border-radius:8px;overflow:hidden;">
                                <tr>
                                    <td style="background-color:#2196F3;padding:30px;text-align:center;">
                                        <h1 style="color:#ffffff;margin:0;font-size:24px;">Sports Gurukul</h1>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="padding:40px 30px;">
                                        <h2 style="color:#333333;margin:0 0 20px;">Password Reset Request</h2>
                                        <p style="color:#555555;font-size:16px;line-height:1.6;margin:0 0 20px;">
                                            Hi {{safeName}},<br><br>
                                            We received a request to reset your password. Click the button below to set a new password.
                                        </p>
                                        <table cellpadding="0" cellspacing="0" style="margin:0 0 20px;">
                                            <tr>
                                                <td style="background-color:#2196F3;border-radius:5px;">
                                                    <a href="{{resetLink}}" style="display:inline-block;padding:14px 30px;color:#ffffff;text-decoration:none;font-size:16px;font-weight:bold;">Reset Password</a>
                                                </td>
                                            </tr>
                                        </table>
                                        <p style="color:#555555;font-size:14px;line-height:1.6;margin:0 0 10px;">
                                            This link expires in <strong>{{expiryMinutes}} minute(s)</strong>.
                                        </p>
                                        <p style="color:#999999;font-size:13px;line-height:1.6;margin:0 0 10px;">
                                            If you did not request a password reset, please ignore this email. Your password will remain unchanged.
                                        </p>
                                        <p style="color:#999999;font-size:13px;line-height:1.6;margin:0;">
                                            If the button above doesn't work, copy and paste this link into your browser:<br>
                                            <a href="{{resetLink}}" style="color:#2196F3;word-break:break-all;">{{resetLink}}</a>
                                        </p>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="background-color:#f4f4f4;padding:20px 30px;text-align:center;">
                                        <p style="color:#999999;font-size:12px;margin:0;">
                                            &copy; {{year}} Sports Gurukul. All rights reserved.
                                        </p>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </body>
            </html>
            """;

        return body;
    }

    private static string EscapeHtml(string input)
    {
        return input
            .Replace("&", "&amp;")
            .Replace("<", "&lt;")
            .Replace(">", "&gt;")
            .Replace("\"", "&quot;")
            .Replace("'", "&#39;");
    }
}
