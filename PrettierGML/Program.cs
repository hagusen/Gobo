﻿using PrettierGML;
using System.Diagnostics;

static string Format(string input)
{
    var formatOptions = new FormatOptions() { BraceStyle = BraceStyle.SameLine, CheckAst = true };

    Stopwatch sw = Stopwatch.StartNew();
    var result = GmlFormatter.Format(input, formatOptions);
    sw.Stop();

    Console.WriteLine(result);
    Console.WriteLine($"Total Time: {sw.ElapsedMilliseconds} ms");

    return result;
}

var input = $$"""
    switch(_charArray[_i])
    {
        //Set up alternating single quote marks
        case ord("'").foo().bar: 
            _inSingleQuote = !_inSingleQuote;
            _charArray[@ _i] = _inSingleQuote? ord("^") : ord("*");
            break;
        default: break
    }
    """;

Format(input);
