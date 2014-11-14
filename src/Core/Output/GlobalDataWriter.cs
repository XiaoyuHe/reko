﻿#region License
/* 
 * Copyright (C) 1999-2014 John Källén.
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2, or (at your option)
 * any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; see the file COPYING.  If not, write to
 * the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.
 */
#endregion

using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Decompiler.Core.Output
{
    /// <summary>
    /// Writes out initialized data.
    /// </summary>
    public class GlobalDataWriter : IDataTypeVisitor<CodeFormatter>
    {
        private Program program;
        private ImageReader rdr;
        private CodeFormatter codeFormatter;
        private StructureType globals;

        public GlobalDataWriter(Program program)
        {
            this.program = program;
        }

        public void WriteGlobals(Formatter formatter)
        {
            this.codeFormatter = new CodeFormatter(formatter);
            var tw = new TypeFormatter(formatter, true);
            this.globals = (StructureType) program.Globals.TypeVariable.DataType;
            foreach (var field in globals.Fields)
            {
                var name = string.Format("g_{0:X}", field.Name);
                tw.Write(field.DataType, name);
                formatter.Write(" = ");
                this.rdr = program.Architecture.CreateImageReader(program.Image, new Address((uint) field.Offset));
                field.DataType.Accept(this);
                formatter.Terminate(";");
            }
        }

        public CodeFormatter VisitArray(ArrayType at)
        {
            Debug.Assert(at.Length != 0, "Expected sizes of arrays to have been determined by now");
            var fmt = codeFormatter.InnerFormatter;
            fmt.Terminate();
            fmt.Indent();
            fmt.Write("{");
            fmt.Terminate();
            fmt.Indentation += fmt.TabSize;

            for (int i = 0; i < at.Length; ++i)
            {
                fmt.Indent();
                at.ElementType.Accept(this);
                fmt.Terminate(",");
            }

            fmt.Indentation -= fmt.TabSize;
            fmt.Indent();
            fmt.Write("}");
            return codeFormatter;
        }

        public CodeFormatter VisitCode(CodeType c)
        {
            throw new NotImplementedException();
        }

        public CodeFormatter VisitEnum(EnumType e)
        {
            throw new NotImplementedException();
        }

        public CodeFormatter VisitEquivalenceClass(EquivalenceClass eq)
        {
            return eq.DataType.Accept(this);
        }

        public CodeFormatter VisitFunctionType(FunctionType ft)
        {
            throw new NotImplementedException();
        }

        public CodeFormatter VisitPrimitive(PrimitiveType pt)
        {
            rdr.Read(pt).Accept(codeFormatter);
            return codeFormatter;
        }

        public CodeFormatter VisitMemberPointer(MemberPointer memptr)
        {
            throw new NotImplementedException();
        }

        public CodeFormatter VisitPointer(Pointer ptr)
        {
            var c = rdr.Read(PrimitiveType.Create(Domain.Pointer, ptr.Size));
            int offset = c.ToInt32();
            if (offset == 0)
            {
                codeFormatter.WriteNull();
            }
            else
            {
                var field = globals.Fields.AtOffset(offset);
                if (field == null)
                    throw new NotImplementedException("Drill into struct");
                codeFormatter.InnerFormatter.Write("&g_{0}", field.Name);
            }
            return codeFormatter;
        }

        public CodeFormatter VisitString(StringType str)
        {
            throw new NotImplementedException();
        }

        public CodeFormatter VisitStructure(StructureType str)
        {
            var fmt = codeFormatter.InnerFormatter;
            fmt.Terminate();
            fmt.Indent();
            fmt.Write("{");
            fmt.Terminate();
            fmt.Indentation += fmt.TabSize;

            for (int i = 0; i < str.Fields.Count; ++i)
            {
                fmt.Indent();
                str.Fields[i].DataType.Accept(this);
                fmt.Terminate(",");
            }

            fmt.Indentation -= fmt.TabSize;
            fmt.Indent();
            fmt.Write("}");
            return codeFormatter;
        }

        public CodeFormatter VisitTypeReference(TypeReference typeref)
        {
            throw new NotImplementedException();
        }

        public CodeFormatter VisitTypeVariable(TypeVariable tv)
        {
            throw new NotImplementedException();
        }

        public CodeFormatter VisitUnion(UnionType ut)
        {
            throw new NotImplementedException();
        }

        public CodeFormatter VisitUnknownType(UnknownType ut)
        {
            throw new NotImplementedException();
        }

        public CodeFormatter VisitVoidType(VoidType voidType)
        {
            throw new NotImplementedException();
        }
    }
}