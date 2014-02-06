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

using Decompiler.Core;
using Decompiler.Core.Expressions;
using Decompiler.Core.Rtl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.Arch.M68k
{
    public partial class Rewriter
    {
        private void RewriteJsr()
        {
            var src = orw.RewriteSrc(di.op1);
            emitter.Call(src, 4);
        }

        private void RewriteDbcc(ConditionCode cc, FlagM flags)
        {
            if (cc != ConditionCode.None)
            {
                emitter.Branch(
                    emitter.Test(cc, orw.FlagGroup(flags)),
                    di.Address + 4,
                    RtlClass.ConditionalTransfer);
            }
            var src = orw.RewriteSrc(di.op1);

            emitter.Assign(src, emitter.ISub(src, 1));
            emitter.Branch(
                emitter.Ne(src, emitter.Int32(-1)),
                (Address) orw.RewriteSrc(di.op2),
                RtlClass.ConditionalTransfer);
        }
    }
}