﻿using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace PrettierGML.Nodes.SyntaxNodes
{
    internal class Identifier : GmlSyntaxNode
    {
        public string Name { get; set; }

        public Identifier(ITerminalNode context, string name)
            : base(context)
        {
            Name = name;
        }

        public Identifier(ParserRuleContext context, string name)
            : base(context)
        {
            Name = name;
        }

        public override Doc Print()
        {
            return Name;
        }
    }
}
