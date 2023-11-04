﻿using Antlr4.Runtime.Misc;
using PrettierGML.SyntaxNodes;
using PrettierGML.SyntaxNodes.Gml;
using PrettierGML.SyntaxNodes.GmlExtensions;
using UnaryExpression = PrettierGML.SyntaxNodes.Gml.UnaryExpression;

namespace PrettierGML.Parser
{
    /// <summary>
    /// Visits the Antlr-generated parse tree and returns a GmlSyntaxNode tree.
    /// </summary>
    internal class GmlAstBuilder : GameMakerLanguageParserBaseVisitor<GmlSyntaxNode>
    {
        public override GmlSyntaxNode VisitProgram(
            [NotNull] GameMakerLanguageParser.ProgramContext context
        )
        {
            if (context.statementList() != null)
            {
                var statements = Visit(context.statementList());
                return new Document(context, statements.Children);
            }
            else
            {
                return GmlSyntaxNode.Empty;
            }
        }

        public override GmlSyntaxNode VisitStatementList(
            [NotNull] GameMakerLanguageParser.StatementListContext context
        )
        {
            var statements = context.statement();
            var parts = new List<GmlSyntaxNode>();

            for (var i = 0; i < statements.Length; i++)
            {
                GmlSyntaxNode statement = Visit(statements[i]);
                if (statement is EmptyNode)
                {
                    continue;
                }
                parts.Add(statement);
            }

            return parts;
        }

        public override GmlSyntaxNode VisitStatement(
            [NotNull] GameMakerLanguageParser.StatementContext context
        )
        {
            if (context.block() != null)
            {
                return Visit(context.block());
            }
            else if (context.ifStatement() != null)
            {
                return Visit(context.ifStatement());
            }
            else if (context.variableDeclarationList() != null)
            {
                return Visit(context.variableDeclarationList());
            }
            else if (context.assignmentExpression() != null)
            {
                return Visit(context.assignmentExpression());
            }
            else if (context.callStatement() != null)
            {
                return Visit(context.callStatement());
            }
            else if (context.iterationStatement() != null)
            {
                return Visit(context.iterationStatement());
            }
            else if (context.functionDeclaration() != null)
            {
                return Visit(context.functionDeclaration());
            }
            else if (context.switchStatement() != null)
            {
                return Visit(context.switchStatement());
            }
            else if (context.enumeratorDeclaration() != null)
            {
                return Visit(context.enumeratorDeclaration());
            }
            else if (context.incDecStatement() != null)
            {
                return Visit(context.incDecStatement());
            }
            else if (context.withStatement() != null)
            {
                return Visit(context.withStatement());
            }
            else if (context.returnStatement() != null)
            {
                return Visit(context.returnStatement());
            }
            else if (context.exitStatement() != null)
            {
                return Visit(context.exitStatement());
            }
            else if (context.continueStatement() != null)
            {
                return Visit(context.continueStatement());
            }
            else if (context.breakStatement() != null)
            {
                return Visit(context.breakStatement());
            }
            else if (context.throwStatement() != null)
            {
                return Visit(context.throwStatement());
            }
            else if (context.tryStatement() != null)
            {
                return Visit(context.tryStatement());
            }
            else if (context.globalVarStatement() != null)
            {
                return Visit(context.globalVarStatement());
            }
            else if (context.macroStatement() != null)
            {
                return Visit(context.macroStatement());
            }
            else if (context.defineStatement() != null)
            {
                return Visit(context.defineStatement());
            }
            else if (context.regionStatement() != null)
            {
                return Visit(context.regionStatement());
            }

            return GmlSyntaxNode.Empty;
        }

        public override GmlSyntaxNode VisitBlock(
            [NotNull] GameMakerLanguageParser.BlockContext context
        )
        {
            GmlSyntaxNode body;

            if (context.statementList() != null)
            {
                body = Visit(context.statementList());
            }
            else
            {
                body = GmlSyntaxNode.Empty;
            }

            return new Block(context, body.Children);
        }

        public override GmlSyntaxNode VisitIfStatement(
            [NotNull] GameMakerLanguageParser.IfStatementContext context
        )
        {
            var test = Visit(context.expression());
            var consequent = Visit(context.statement()[0]);
            GmlSyntaxNode alternate = GmlSyntaxNode.Empty;

            if (context.statement().Length > 1)
            {
                alternate = Visit(context.statement()[1]);
            }

            return new IfStatement(context, test, consequent, alternate);
        }

        public override GmlSyntaxNode VisitDoStatement(
            [NotNull] GameMakerLanguageParser.DoStatementContext context
        )
        {
            var body = Visit(context.statement());
            var test = Visit(context.expression());
            return new DoStatement(context, body, test);
        }

        public override GmlSyntaxNode VisitWhileStatement(
            [NotNull] GameMakerLanguageParser.WhileStatementContext context
        )
        {
            var test = Visit(context.expression());
            var body = Visit(context.statement());
            return new WhileStatement(context, test, body);
        }

        public override GmlSyntaxNode VisitRepeatStatement(
            [NotNull] GameMakerLanguageParser.RepeatStatementContext context
        )
        {
            var test = Visit(context.expression());
            var body = Visit(context.statement());
            return new RepeatStatement(context, test, body);
        }

        public override GmlSyntaxNode VisitForStatement(
            [NotNull] GameMakerLanguageParser.ForStatementContext context
        )
        {
            GmlSyntaxNode init = GmlSyntaxNode.Empty;
            GmlSyntaxNode test = GmlSyntaxNode.Empty;
            GmlSyntaxNode update = GmlSyntaxNode.Empty;
            GmlSyntaxNode body;

            if (context.variableDeclarationList() != null)
            {
                init = Visit(context.variableDeclarationList());
            }
            else if (context.assignmentExpression() != null)
            {
                init = Visit(context.assignmentExpression());
            }

            if (context.expression() != null)
            {
                test = Visit(context.expression());
            }

            if (context.statement().Length > 1)
            {
                update = Visit(context.statement()[0]);
                body = Visit(context.statement()[1]);
            }
            else
            {
                body = Visit(context.statement()[0]);
            }

            return new ForStatement(context, init, test, update, body);
        }

        public override GmlSyntaxNode VisitWithStatement(
            [NotNull] GameMakerLanguageParser.WithStatementContext context
        )
        {
            var @object = Visit(context.expression());
            var body = Visit(context.statement());
            return new WithStatement(context, @object, body);
        }

        public override GmlSyntaxNode VisitSwitchStatement(
            [NotNull] GameMakerLanguageParser.SwitchStatementContext context
        )
        {
            var discriminant = Visit(context.expression());
            var cases = Visit(context.caseBlock());
            return new SwitchStatement(context, discriminant, cases);
        }

        public override GmlSyntaxNode VisitCaseBlock(
            [NotNull] GameMakerLanguageParser.CaseBlockContext context
        )
        {
            var caseClauses = new List<GmlSyntaxNode>();
            if (context.caseClauses() != null)
            {
                foreach (var caseList in context.caseClauses())
                {
                    caseClauses.AddRange(Visit(caseList).Children);
                }
            }
            if (context.defaultClause() != null)
            {
                caseClauses.Add(Visit(context.defaultClause()));
            }
            return new SwitchBlock(context, caseClauses);
        }

        public override GmlSyntaxNode VisitCaseClauses(
            [NotNull] GameMakerLanguageParser.CaseClausesContext context
        )
        {
            var parts = new List<GmlSyntaxNode>();
            foreach (var caseClause in context.caseClause())
            {
                parts.Add(Visit(caseClause));
            }
            return parts;
        }

        public override GmlSyntaxNode VisitCaseClause(
            [NotNull] GameMakerLanguageParser.CaseClauseContext context
        )
        {
            var test = Visit(context.expression());
            GmlSyntaxNode body = GmlSyntaxNode.Empty;
            if (context.statementList() != null)
            {
                var sectionContext = context.statementList();
                var statements = Visit(context.statementList());
                body = new SwitchCaseBody(sectionContext, statements.Children);
            }
            return new SwitchCase(context, test, body);
        }

        public override GmlSyntaxNode VisitDefaultClause(
            [NotNull] GameMakerLanguageParser.DefaultClauseContext context
        )
        {
            GmlSyntaxNode body = GmlSyntaxNode.Empty;
            if (context.statementList() != null)
            {
                var sectionContext = context.statementList();
                var statements = Visit(context.statementList());
                body = new SwitchCaseBody(sectionContext, statements.Children);
            }
            return new SwitchCase(context, GmlSyntaxNode.Empty, body);
        }

        public override GmlSyntaxNode VisitContinueStatement(
            [NotNull] GameMakerLanguageParser.ContinueStatementContext context
        )
        {
            return new ContinueStatement(context);
        }

        public override GmlSyntaxNode VisitBreakStatement(
            [NotNull] GameMakerLanguageParser.BreakStatementContext context
        )
        {
            return new BreakStatement(context);
        }

        public override GmlSyntaxNode VisitExitStatement(
            [NotNull] GameMakerLanguageParser.ExitStatementContext context
        )
        {
            return new ExitStatement(context);
        }

        public override GmlSyntaxNode VisitReturnStatement(
            [NotNull] GameMakerLanguageParser.ReturnStatementContext context
        )
        {
            GmlSyntaxNode expression = GmlSyntaxNode.Empty;
            if (context.expression() != null)
            {
                expression = Visit(context.expression());
            }
            return new ReturnStatement(context, expression);
        }

        public override GmlSyntaxNode VisitAssignmentExpression(
            [NotNull] GameMakerLanguageParser.AssignmentExpressionContext context
        )
        {
            var left = Visit(context.lValueExpression());
            var right = Visit(context.expressionOrFunction());
            GmlSyntaxNode type = GmlSyntaxNode.Empty;

            var @operator = context.assignmentOperator().GetText();
            if (@operator == ":=")
            {
                @operator = "=";
            }

            if (context.typeAnnotation() != null)
            {
                type = Visit(context.typeAnnotation());
            }
            return new AssignmentExpression(context, @operator, left, right, type);
        }

        public override GmlSyntaxNode VisitVariableDeclarationList(
            [NotNull] GameMakerLanguageParser.VariableDeclarationListContext context
        )
        {
            var declarations = new List<GmlSyntaxNode>();
            foreach (var declaration in context.variableDeclaration())
            {
                declarations.Add(Visit(declaration));
            }
            var kind = context.varModifier().GetText();
            return new VariableDeclarationList(context, declarations, kind);
        }

        public override GmlSyntaxNode VisitVariableDeclaration(
            [NotNull] GameMakerLanguageParser.VariableDeclarationContext context
        )
        {
            var id = Visit(context.identifier());
            GmlSyntaxNode initializer = GmlSyntaxNode.Empty;
            GmlSyntaxNode type = GmlSyntaxNode.Empty;

            if (context.expressionOrFunction() != null)
            {
                initializer = Visit(context.expressionOrFunction());
            }
            if (context.typeAnnotation() != null)
            {
                type = Visit(context.typeAnnotation());
            }

            return new VariableDeclarator(context, id, type, initializer);
        }

        public override GmlSyntaxNode VisitTypeAnnotation(
            [NotNull] GameMakerLanguageParser.TypeAnnotationContext context
        )
        {
            var types = new List<string>();

            foreach (var type in context.identifier())
            {
                types.Add(type.GetText());
            }

            return new TypeAnnotation(context, types);
        }

        public override GmlSyntaxNode VisitFunctionDeclaration(
            [NotNull] GameMakerLanguageParser.FunctionDeclarationContext context
        )
        {
            GmlSyntaxNode id = GmlSyntaxNode.Empty;
            var parameters = Visit(context.parameterList());
            var body = Visit(context.block());
            GmlSyntaxNode constructorClause = GmlSyntaxNode.Empty;

            if (context.Identifier() != null)
            {
                id = new Identifier(context.Identifier(), context.Identifier().GetText());
            }

            if (context.constructorClause() != null)
            {
                constructorClause = Visit(context.constructorClause());
            }

            return new FunctionDeclaration(context, id, parameters, body, constructorClause);
        }

        public override GmlSyntaxNode VisitConstructorClause(
            [NotNull] GameMakerLanguageParser.ConstructorClauseContext context
        )
        {
            GmlSyntaxNode id = GmlSyntaxNode.Empty;
            GmlSyntaxNode parameters = GmlSyntaxNode.Empty;
            if (context.parameterList() != null)
            {
                parameters = Visit(context.parameterList());
            }
            if (context.Identifier() != null)
            {
                id = new Identifier(context.Identifier(), context.Identifier().GetText());
            }
            return new ConstructorClause(context, id, parameters);
        }

        public override GmlSyntaxNode VisitParameterList(
            [NotNull] GameMakerLanguageParser.ParameterListContext context
        )
        {
            var parts = new List<GmlSyntaxNode>();
            foreach (var arg in context.parameterArgument())
            {
                parts.Add(Visit(arg));
            }
            return new ParameterList(context, parts);
        }

        public override GmlSyntaxNode VisitParameterArgument(
            [NotNull] GameMakerLanguageParser.ParameterArgumentContext context
        )
        {
            var name = Visit(context.identifier());
            GmlSyntaxNode type = GmlSyntaxNode.Empty;
            GmlSyntaxNode initializer = GmlSyntaxNode.Empty;

            if (context.expressionOrFunction() != null)
            {
                initializer = Visit(context.expressionOrFunction());
            }
            if (context.typeAnnotation() != null)
            {
                type = Visit(context.typeAnnotation());
            }

            return new Parameter(context, name, type, initializer);
        }

        public override GmlSyntaxNode VisitTemplateStringLiteral(
            [NotNull] GameMakerLanguageParser.TemplateStringLiteralContext context
        )
        {
            var parts = new List<GmlSyntaxNode>();
            foreach (var atom in context.templateStringAtom())
            {
                parts.Add(Visit(atom));
            }
            return new TemplateStringLiteral(context, parts);
        }

        public override GmlSyntaxNode VisitTemplateStringAtom(
            [NotNull] GameMakerLanguageParser.TemplateStringAtomContext context
        )
        {
            if (context.expression() != null)
            {
                return new TemplateExpression(context, Visit(context.expression()));
            }
            else
            {
                return new TemplateText(context, context.GetText());
            }
        }

        public override GmlSyntaxNode VisitLiteral(
            [NotNull] GameMakerLanguageParser.LiteralContext context
        )
        {
            if (context.arrayLiteral() != null)
            {
                return Visit(context.arrayLiteral());
            }
            else if (context.structLiteral() != null)
            {
                return Visit(context.structLiteral());
            }
            else if (context.templateStringLiteral() != null)
            {
                return Visit(context.templateStringLiteral());
            }
            else
            {
                return new Literal(context, context.GetText());
            }
        }

        public override GmlSyntaxNode VisitLiteralExpression(
            [NotNull] GameMakerLanguageParser.LiteralExpressionContext context
        )
        {
            return Visit(context.literal());
        }

        public override GmlSyntaxNode VisitExpressionOrFunction(
            [NotNull] GameMakerLanguageParser.ExpressionOrFunctionContext context
        )
        {
            GmlSyntaxNode contents;
            if (context.expression() != null)
            {
                contents = Visit(context.expression());
            }
            else if (context.functionDeclaration() != null)
            {
                contents = Visit(context.functionDeclaration());
            }
            else if (context.expressionOrFunction() != null)
            {
                contents = new ParenthesizedExpression(
                    context,
                    Visit(context.expressionOrFunction())
                );
            }
            else
            {
                return GmlSyntaxNode.Empty;
            }

            return contents;
        }

        public override GmlSyntaxNode VisitLValueStartExpression(
            [NotNull] GameMakerLanguageParser.LValueStartExpressionContext context
        )
        {
            if (context.identifier() != null)
            {
                return Visit(context.identifier());
            }
            else if (context.expressionOrFunction() != null)
            {
                return new ParenthesizedExpression(context, Visit(context.expressionOrFunction()));
            }
            else
            {
                return Visit(context.newExpression());
            }
        }

        public override GmlSyntaxNode VisitLValueExpression(
            [NotNull] GameMakerLanguageParser.LValueExpressionContext context
        )
        {
            GmlSyntaxNode @object = Visit(context.lValueStartExpression());

            if (context.lValueChainOperator()?.Length > 0)
            {
                var ops = context.lValueChainOperator();
                foreach (var op in ops)
                {
                    var node = Visit(op);
                    (node as IHasObject)!.Object = @object;
                    @object = node;
                }
            }

            if (context.lValueFinalOperator() != null)
            {
                var node = Visit(context.lValueFinalOperator());
                (node as IHasObject)!.Object = @object;
                @object = node;
            }

            return @object;
        }

        public override GmlSyntaxNode VisitArguments(
            [NotNull] GameMakerLanguageParser.ArgumentsContext context
        )
        {
            var parts = new List<GmlSyntaxNode>();
            foreach (var expressionContext in context.expressionOrFunction())
            {
                var arg = Visit(expressionContext);
                parts.Add(arg);
            }
            return new ArgumentList(context, parts);
        }

        public override GmlSyntaxNode VisitCallExpression(
            [NotNull] GameMakerLanguageParser.CallExpressionContext context
        )
        {
            return Visit(context.callStatement());
        }

        public override GmlSyntaxNode VisitCallStatement(
            [NotNull] GameMakerLanguageParser.CallStatementContext context
        )
        {
            GmlSyntaxNode @object = GmlSyntaxNode.Empty;
            if (context.callableExpression() != null)
            {
                @object = Visit(context.callableExpression());
            }
            if (context.callStatement() != null)
            {
                @object = Visit(context.callStatement());
            }
            return new CallExpression(context, @object, Visit(context.arguments()));
        }

        public override GmlSyntaxNode VisitCallLValue(
            [NotNull] GameMakerLanguageParser.CallLValueContext context
        )
        {
            return new CallExpression(context, GmlSyntaxNode.Empty, Visit(context.arguments()));
        }

        public override GmlSyntaxNode VisitCallableExpression(
            [NotNull] GameMakerLanguageParser.CallableExpressionContext context
        )
        {
            if (context.lValueExpression() != null)
            {
                return Visit(context.lValueExpression());
            }
            if (context.functionDeclaration() != null)
            {
                return Visit(context.functionDeclaration());
            }
            if (context.callableExpression() != null)
            {
                return Visit(context.callableExpression());
            }
            return GmlSyntaxNode.Empty;
        }

        public override GmlSyntaxNode VisitNewExpression(
            [NotNull] GameMakerLanguageParser.NewExpressionContext context
        )
        {
            var name = context.identifier() is null
                ? GmlSyntaxNode.Empty
                : Visit(context.identifier());
            var arguments = Visit(context.arguments());
            return new NewExpression(context, name, arguments);
        }

        public override GmlSyntaxNode VisitMemberDotLValue(
            [NotNull] GameMakerLanguageParser.MemberDotLValueContext context
        )
        {
            var property = Visit(context.identifier());
            return new MemberDotExpression(context, GmlSyntaxNode.Empty, property);
        }

        public override GmlSyntaxNode VisitMemberDotLValueFinal(
            [NotNull] GameMakerLanguageParser.MemberDotLValueFinalContext context
        )
        {
            var property = Visit(context.identifier());
            return new MemberDotExpression(context, GmlSyntaxNode.Empty, property);
        }

        public override GmlSyntaxNode VisitMemberIndexLValue(
            [NotNull] GameMakerLanguageParser.MemberIndexLValueContext context
        )
        {
            var property = Visit(context.expressionSequence());
            var accessor = context.accessor().GetText();
            return new MemberIndexExpression(context, GmlSyntaxNode.Empty, property, accessor);
        }

        public override GmlSyntaxNode VisitMemberIndexLValueFinal(
            [NotNull] GameMakerLanguageParser.MemberIndexLValueFinalContext context
        )
        {
            var property = Visit(context.expressionSequence());
            var accessor = context.accessor().GetText();
            return new MemberIndexExpression(context, GmlSyntaxNode.Empty, property, accessor);
        }

        public override GmlSyntaxNode VisitExpressionSequence(
            [NotNull] GameMakerLanguageParser.ExpressionSequenceContext context
        )
        {
            var parts = new List<GmlSyntaxNode>();
            foreach (var expression in context.expression())
            {
                parts.Add(Visit(expression));
            }
            return parts;
        }

        public override GmlSyntaxNode VisitParenthesizedExpression(
            [NotNull] GameMakerLanguageParser.ParenthesizedExpressionContext context
        )
        {
            var content = Visit(context.expression());
            return new ParenthesizedExpression(context, content);
        }

        public override GmlSyntaxNode VisitArrayLiteral(
            [NotNull] GameMakerLanguageParser.ArrayLiteralContext context
        )
        {
            if (context.elementList() == null)
            {
                return new ArrayExpression(context, new());
            }

            var elementNodes = new List<GmlSyntaxNode>();
            foreach (var element in context.elementList().expressionOrFunction())
            {
                elementNodes.Add(Visit(element));
            }

            return new ArrayExpression(context, elementNodes);
        }

        public override GmlSyntaxNode VisitStructLiteral(
            [NotNull] GameMakerLanguageParser.StructLiteralContext context
        )
        {
            var propertyNodes = new List<GmlSyntaxNode>();
            foreach (var property in context.propertyAssignment())
            {
                propertyNodes.Add(Visit(property));
            }
            return new StructExpression(context, propertyNodes);
        }

        public override GmlSyntaxNode VisitPropertyAssignment(
            [NotNull] GameMakerLanguageParser.PropertyAssignmentContext context
        )
        {
            var name = new Identifier(
                context.propertyIdentifier(),
                context.propertyIdentifier().GetText()
            );
            var expression = Visit(context.expressionOrFunction());
            return new StructProperty(context, name, expression);
        }

        public override GmlSyntaxNode VisitIdentifier(
            [NotNull] GameMakerLanguageParser.IdentifierContext context
        )
        {
            return new Identifier(context, context.GetText());
        }

        public override GmlSyntaxNode VisitEnumeratorDeclaration(
            [NotNull] GameMakerLanguageParser.EnumeratorDeclarationContext context
        )
        {
            var name = Visit(context.identifier());
            GmlSyntaxNode members = GmlSyntaxNode.Empty;
            if (context.enumeratorList() != null)
            {
                members = Visit(context.enumeratorList());
            }
            return new EnumDeclaration(context, name, members);
        }

        public override GmlSyntaxNode VisitEnumeratorList(
            [NotNull] GameMakerLanguageParser.EnumeratorListContext context
        )
        {
            var declarations = new List<GmlSyntaxNode>();
            foreach (var enumDecl in context.enumerator())
            {
                declarations.Add(Visit(enumDecl));
            }
            return new EnumBlock(context, declarations);
        }

        public override GmlSyntaxNode VisitEnumerator(
            [NotNull] GameMakerLanguageParser.EnumeratorContext context
        )
        {
            var id = Visit(context.identifier());
            GmlSyntaxNode initializer = GmlSyntaxNode.Empty;
            if (context.Assign() != null)
            {
                if (context.IntegerLiteral() != null)
                {
                    initializer = new Literal(
                        context.IntegerLiteral(),
                        context.IntegerLiteral().GetText()
                    );
                }
                if (context.HexIntegerLiteral() != null)
                {
                    initializer = new Literal(
                        context.HexIntegerLiteral(),
                        context.HexIntegerLiteral().GetText()
                    );
                }
                if (context.BinaryLiteral() != null)
                {
                    initializer = new Literal(
                        context.BinaryLiteral(),
                        context.BinaryLiteral().GetText()
                    );
                }
            }
            return new EnumMember(context, id, initializer);
        }

        public override GmlSyntaxNode VisitMultiplicativeExpression(
            [NotNull] GameMakerLanguageParser.MultiplicativeExpressionContext context
        )
        {
            var expressions = context.GetRuleContexts<GameMakerLanguageParser.ExpressionContext>();
            var @operator = context.children[1].GetText();
            return new BinaryExpression(
                context,
                @operator,
                Visit(expressions[0]),
                Visit(expressions[1])
            );
        }

        public override GmlSyntaxNode VisitAdditiveExpression(
            [NotNull] GameMakerLanguageParser.AdditiveExpressionContext context
        )
        {
            var expressions = context.GetRuleContexts<GameMakerLanguageParser.ExpressionContext>();
            var @operator = context.children[1].GetText();
            return new BinaryExpression(
                context,
                @operator,
                Visit(expressions[0]),
                Visit(expressions[1])
            );
        }

        public override GmlSyntaxNode VisitCoalesceExpression(
            [NotNull] GameMakerLanguageParser.CoalesceExpressionContext context
        )
        {
            var expressions = context.GetRuleContexts<GameMakerLanguageParser.ExpressionContext>();
            var @operator = context.children[1].GetText();
            return new BinaryExpression(
                context,
                @operator,
                Visit(expressions[0]),
                Visit(expressions[1])
            );
        }

        public override GmlSyntaxNode VisitBitShiftExpression(
            [NotNull] GameMakerLanguageParser.BitShiftExpressionContext context
        )
        {
            var expressions = context.GetRuleContexts<GameMakerLanguageParser.ExpressionContext>();
            var @operator = context.children[1].GetText();
            return new BinaryExpression(
                context,
                @operator,
                Visit(expressions[0]),
                Visit(expressions[1])
            );
        }

        public override GmlSyntaxNode VisitLogicalOrExpression(
            [NotNull] GameMakerLanguageParser.LogicalOrExpressionContext context
        )
        {
            var expressions = context.GetRuleContexts<GameMakerLanguageParser.ExpressionContext>();
            var @operator = context.children[1].GetText();
            return new BinaryExpression(
                context,
                @operator,
                Visit(expressions[0]),
                Visit(expressions[1])
            );
        }

        public override GmlSyntaxNode VisitLogicalAndExpression(
            [NotNull] GameMakerLanguageParser.LogicalAndExpressionContext context
        )
        {
            var expressions = context.GetRuleContexts<GameMakerLanguageParser.ExpressionContext>();
            var @operator = context.children[1].GetText();
            return new BinaryExpression(
                context,
                @operator,
                Visit(expressions[0]),
                Visit(expressions[1])
            );
        }

        public override GmlSyntaxNode VisitLogicalXorExpression(
            [NotNull] GameMakerLanguageParser.LogicalXorExpressionContext context
        )
        {
            var expressions = context.GetRuleContexts<GameMakerLanguageParser.ExpressionContext>();
            var @operator = context.children[1].GetText();
            return new BinaryExpression(
                context,
                @operator,
                Visit(expressions[0]),
                Visit(expressions[1])
            );
        }

        public override GmlSyntaxNode VisitEqualityExpression(
            [NotNull] GameMakerLanguageParser.EqualityExpressionContext context
        )
        {
            var expressions = context.GetRuleContexts<GameMakerLanguageParser.ExpressionContext>();
            var @operator = context.children[1].GetText();
            return new BinaryExpression(
                context,
                @operator,
                Visit(expressions[0]),
                Visit(expressions[1])
            );
        }

        public override GmlSyntaxNode VisitInequalityExpression(
            [NotNull] GameMakerLanguageParser.InequalityExpressionContext context
        )
        {
            var expressions = context.GetRuleContexts<GameMakerLanguageParser.ExpressionContext>();
            var @operator = context.children[1].GetText();
            return new BinaryExpression(
                context,
                @operator,
                Visit(expressions[0]),
                Visit(expressions[1])
            );
        }

        public override GmlSyntaxNode VisitRelationalExpression(
            [NotNull] GameMakerLanguageParser.RelationalExpressionContext context
        )
        {
            var expressions = context.GetRuleContexts<GameMakerLanguageParser.ExpressionContext>();
            var @operator = context.children[1].GetText();
            return new BinaryExpression(
                context,
                @operator,
                Visit(expressions[0]),
                Visit(expressions[1])
            );
        }

        public override GmlSyntaxNode VisitBitAndExpression(
            [NotNull] GameMakerLanguageParser.BitAndExpressionContext context
        )
        {
            var expressions = context.GetRuleContexts<GameMakerLanguageParser.ExpressionContext>();
            var @operator = context.children[1].GetText();
            return new BinaryExpression(
                context,
                @operator,
                Visit(expressions[0]),
                Visit(expressions[1])
            );
        }

        public override GmlSyntaxNode VisitBitOrExpression(
            [NotNull] GameMakerLanguageParser.BitOrExpressionContext context
        )
        {
            var expressions = context.GetRuleContexts<GameMakerLanguageParser.ExpressionContext>();
            var @operator = context.children[1].GetText();
            return new BinaryExpression(
                context,
                @operator,
                Visit(expressions[0]),
                Visit(expressions[1])
            );
        }

        public override GmlSyntaxNode VisitBitXOrExpression(
            [NotNull] GameMakerLanguageParser.BitXOrExpressionContext context
        )
        {
            var expressions = context.GetRuleContexts<GameMakerLanguageParser.ExpressionContext>();
            var @operator = context.children[1].GetText();
            return new BinaryExpression(
                context,
                @operator,
                Visit(expressions[0]),
                Visit(expressions[1])
            );
        }

        public override GmlSyntaxNode VisitUnaryMinusExpression(
            [NotNull] GameMakerLanguageParser.UnaryMinusExpressionContext context
        )
        {
            var expression = context.expression();
            return new UnaryExpression(context, "-", Visit(expression), true);
        }

        public override GmlSyntaxNode VisitNotExpression(
            [NotNull] GameMakerLanguageParser.NotExpressionContext context
        )
        {
            var expression = context.expression();
            return new UnaryExpression(context, "!", Visit(expression), true);
        }

        public override GmlSyntaxNode VisitBitNotExpression(
            [NotNull] GameMakerLanguageParser.BitNotExpressionContext context
        )
        {
            var expression = context.expression();
            return new UnaryExpression(context, "~", Visit(expression), true);
        }

        public override GmlSyntaxNode VisitIncDecExpression(
            [NotNull] GameMakerLanguageParser.IncDecExpressionContext context
        )
        {
            return Visit(context.incDecStatement());
        }

        public override GmlSyntaxNode VisitPreIncDecExpression(
            [NotNull] GameMakerLanguageParser.PreIncDecExpressionContext context
        )
        {
            var expression = context.lValueExpression();
            var @operator = context.children[0].GetText();
            return new IncDecStatement(context, @operator, Visit(expression), true);
        }

        public override GmlSyntaxNode VisitPostIncDecExpression(
            [NotNull] GameMakerLanguageParser.PostIncDecExpressionContext context
        )
        {
            var expression = context.lValueExpression();
            var @operator = context.children[1].GetText();
            return new IncDecStatement(context, @operator, Visit(expression), false);
        }

        public override GmlSyntaxNode VisitTernaryExpression(
            [NotNull] GameMakerLanguageParser.TernaryExpressionContext context
        )
        {
            var test = context.expression()[0];
            var whenTrue = context.expression()[1];
            var whenFalse = context.expression()[2];
            return new ConditionalExpression(
                context,
                Visit(test),
                Visit(whenTrue),
                Visit(whenFalse)
            );
        }

        public override GmlSyntaxNode VisitMacroStatement(
            [NotNull] GameMakerLanguageParser.MacroStatementContext context
        )
        {
            var id = Visit(context.identifier());
            var body = "";

            if (context.macroToken().Length > 0)
            {
                var firstToken = context.macroToken().First().Start;
                var lastToken = context.macroToken().Last().Stop;
                var source = firstToken.TokenSource;

                body = source.InputStream.GetText(
                    new Interval(firstToken.StartIndex, lastToken.StopIndex)
                );
            }

            return new MacroDeclaration(context, id, body);
        }

        public override GmlSyntaxNode VisitRegionStatement(
            [NotNull] GameMakerLanguageParser.RegionStatementContext context
        )
        {
            var isEndRegion = context.EndRegion() != null;
            if (isEndRegion)
            {
                return new RegionStatement(context, null, true);
            }
            else
            {
                var name = context.RegionCharacters().GetText();
                return new RegionStatement(context, name, false);
            }
        }

        public override GmlSyntaxNode VisitThrowStatement(
            [NotNull] GameMakerLanguageParser.ThrowStatementContext context
        )
        {
            return new ThrowStatement(context, Visit(context.expression()));
        }

        public override GmlSyntaxNode VisitTryStatement(
            [NotNull] GameMakerLanguageParser.TryStatementContext context
        )
        {
            var finallyProduction = context.finallyProduction() is null
                ? GmlSyntaxNode.Empty
                : Visit(context.finallyProduction());

            return new TryStatement(
                context,
                Visit(context.statement()),
                Visit(context.catchProduction()),
                finallyProduction
            );
        }

        public override GmlSyntaxNode VisitCatchProduction(
            [NotNull] GameMakerLanguageParser.CatchProductionContext context
        )
        {
            GmlSyntaxNode id = GmlSyntaxNode.Empty;
            var body = Visit(context.statement());
            if (context.identifier() != null)
            {
                id = Visit(context.identifier());
            }
            return new CatchProduction(context, id, body);
        }

        public override GmlSyntaxNode VisitFinallyProduction(
            [NotNull] GameMakerLanguageParser.FinallyProductionContext context
        )
        {
            return new FinallyProduction(context, Visit(context.statement()));
        }
    }
}
