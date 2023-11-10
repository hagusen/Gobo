﻿using Antlr4.Runtime;
using PrettierGML.Parser;
using PrettierGML.Printer.DocPrinter;
using PrettierGML.Printer.Utilities;
using PrettierGML.SyntaxNodes;
using System.Diagnostics;

namespace PrettierGML
{
    public struct FormatResult
    {
        public string Output;

        public string Ast = string.Empty;
        public string DocTree = string.Empty;

        public double? ParseTimeMs = null;
        public double? FormatTimeMs = null;
        public double? TotalTimeMs = null;

        public FormatResult(string output)
        {
            Output = output;
        }

        public override readonly string ToString()
        {
            var result = "";

            if (Ast != string.Empty)
                result += $"--- ABSTRACT SYNTAX TREE ---\n\n{Ast}\n\n";

            if (DocTree != string.Empty)
                result += $"--- DOC TREE ---\n\n{DocTree}\n\n";

            result += $"--- PRINTED OUTPUT ---\n{Output}\n\n";

            if (ParseTimeMs != null)
                result += $"Parse: {ParseTimeMs} ms\n";

            if (FormatTimeMs != null)
                result += $"Format: {FormatTimeMs} ms\n";

            if (TotalTimeMs != null)
                result += $"Total: {TotalTimeMs} ms\n";

            return result;
        }
    }

    public static partial class GmlFormatter
    {
        public static FormatResult Format(string code, FormatOptions options)
        {
            code = code.ReplaceLineEndings();

            long parseStart = 0;
            long parseStop = 0;
            long formatStart = 0;

            var getDebugInfo = options.GetDebugInfo;

            if (getDebugInfo)
            {
                parseStart = Stopwatch.GetTimestamp();
            }

            var parseResult = GmlParser.Parse(code);

            if (getDebugInfo)
            {
                parseStop = Stopwatch.GetTimestamp();
                formatStart = Stopwatch.GetTimestamp();
            }

            GmlSyntaxNode ast = parseResult.Ast;
            CommonTokenStream tokens = parseResult.TokenStream;

            var initialHash = options.ValidateOutput ? ast.GetHashCode() : -1;
            var docs = ast.Print(new PrintContext(options, tokens));

            var printOptions = new Printer.DocPrinterOptions()
            {
                Width = options.Width,
                TabWidth = options.TabWidth,
                UseTabs = options.UseTabs,
            };

            var output = DocPrinter.Print(docs, printOptions, Environment.NewLine);

            if (options.ValidateOutput)
            {
                ast.EnsureCommentsPrinted();

                GmlParseResult updatedParseResult;

                try
                {
                    updatedParseResult = GmlParser.Parse(output);
                }
                catch (GmlSyntaxErrorException ex)
                {
                    Console.WriteLine(output);
                    throw new Exception(
                        "Formatting made the code invalid!\nParse error:\n" + ex.Message
                    );
                }

                var resultHash = updatedParseResult.Ast.GetHashCode();

                if (initialHash != resultHash)
                {
                    Console.WriteLine("Original AST:");
                    Console.WriteLine(ast);
                    Console.WriteLine("Formatted AST:");
                    Console.WriteLine(updatedParseResult.Ast);
                    var diff = StringDiffer.PrintFirstDifference(
                        ast.ToString(),
                        updatedParseResult.Ast.ToString()
                    );
                    throw new Exception($"Formatting transformed the AST:\n{diff}");
                }
            }

            if (getDebugInfo)
            {
                long formatStop = Stopwatch.GetTimestamp();
                return new FormatResult(output)
                {
                    Ast = ast.ToString(),
                    DocTree = docs.ToString(),
                    ParseTimeMs = Stopwatch.GetElapsedTime(parseStart, parseStop).TotalMilliseconds,
                    FormatTimeMs = Stopwatch
                        .GetElapsedTime(formatStart, formatStop)
                        .TotalMilliseconds,
                    TotalTimeMs = Stopwatch.GetElapsedTime(parseStart, formatStop).TotalMilliseconds
                };
            }
            else
            {
                return new FormatResult(output);
            }
        }

        public static async Task FormatFileAsync(string filePath, FormatOptions options)
        {
            string input = await File.ReadAllTextAsync(filePath);
            string formatted;

            try
            {
                var result = Format(input, options);
                formatted = result.Output;
            }
            catch (Exception)
            {
                return;
            }

            await File.WriteAllTextAsync(filePath, formatted);
        }
    }
}
