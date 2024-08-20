# RandomPasswordBuilder

## Description

This is just another .NET library generating random strings.

The library was developed as a side product.

It includes the class [DrLSDee.Text.RandomPasswordBuilder.PasswordBuilder](RandomPasswordBuilder\RandomPasswordBuilder\PasswordBuilder.cs) providing a method to generate passwords that are guaranteed to include all specified character categories.

Unlike some others, the class method `PasswordBuilder.Next()` does not simply repeat random string generation until one finally meets the password requirements.

An instance uses a dictionary, where keys are character categories and values are character sets corresponding to those categories.

When generating next random string, the method takes from 1 to N characters from each category, stores in the list and, finally, shuffles the list and builds a string.

This way, for a single password that is guaranteed to meet the requirements, the method is always called only once.

Character categories are:
* Decimal digits from `0` to `9`;
* Latin uppercase letters from `A` to `Z`;
* Latin lowercase letters from `a` to `z`'
* And special printable characters such as dot, comma, slashes, parentheses etc.: `'-!"#$%&()*,./:;?@[]^_{|}~+<=>`

The list of special characters is taken from [this Microsoft article](https://learn.microsoft.com/en-us/previous-versions/windows/it-pro/windows-10/security/threat-protection/security-policy-settings/password-must-meet-complexity-requirements)

Optionally, you can specify lists of additional characters to add or exclude, or both.

The class has a flag that excludes XML-unsafe characters: `"'<>&`; you can use it if you plan to use generated strings in XML documents, e.g. when sending SOAP requests.

## Limitations

The library currently uses .NET Framework 4.8, not .NET or .NET Core.

This is because the core product is being developed for use on general Windows/Windows Server installations where newer runtimes may not be installed.

But feel free to fork this project and adapt it for runtime you prefer.

## Examples

You can also see an example of usage in the PowerShell cmdlet class [GetRandomPasswordCmdlet.cs](RandomPasswordBuilder\RandomPasswordBuilder\Cmdlets\GetRandomPasswordCmdlet.cs)

```csharp
using DrLSDee.Text.RandomPasswordBuilder;
using System;

namespace Com.Contoso.Example
{
    public static class TestClass
    {
        public static int Count { get; set; } = 10;
        public static void Main()
        {
            using (PasswordBuilder builder = new PasswordBuilder())
            {
                for (int i = 0; i < Count; i++)
                {
                    Console.WriteLine($"Password {i + 1} of {Count - 1}: {builder.Next()}");
                }
            }
        }
    }
}
```

It shows something like this:

```
Password 1 of 9: 164Ls:7jT
Password 2 of 9: 84k8N50Q~
Password 3 of 9: VB26>040~s]q
Password 4 of 9: 2U:kEgS7V
Password 5 of 9: sPr_N9VBM9qjaL[
Password 6 of 9: 1cKGAE~MH6}j1;WC
Password 7 of 9: f3?CC7GHE
Password 8 of 9: 3bQd22;3R761pn~u
Password 9 of 9: KJTUO13z4EH3H(t
Password 10 of 9: #FWs3-5v7.558
```

**This** example is not quite correct, because when the static method `Main()` completes, it **disposes** a temporary instance of `PasswordBuilder`.

And because both the class and the method are static and do not provide the way to re-create an instance of `PasswordBuilder`, you can only call the `Main()` once.

So be careful with disposable objects.