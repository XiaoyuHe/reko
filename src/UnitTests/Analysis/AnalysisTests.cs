#region License
/* 
 * Copyright (C) 1999-2016 John K�ll�n.
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

using NUnit.Framework;
using Reko.Analysis;
using Reko.Core;
using Reko.Core.Lib;

namespace Reko.UnitTests.Analysis
{
	[TestFixture]
    [Ignore("SSA transform now incorporates this")]
    public class AnalysisTests : AnalysisTestBase
	{
		[Test]
        public void DiamondDominatorTest()
        {
			Program program = RewriteFile("Fragments/diamond.asm");
			Procedure proc = program.Procedures.Values[0];
			BlockDominatorGraph doms = proc.CreateBlockDominatorGraph();
			var diamondTop = proc.ControlGraph.Blocks[2];
            Assert.AreSame(diamondTop, doms.ImmediateDominator(diamondTop.ElseBlock));
            Assert.AreSame(diamondTop, doms.ImmediateDominator(diamondTop.ThenBlock));
		}

		[Test]
		public void LoopDominatorTest()
		{
			Program program = RewriteFile("Fragments/while_loop.asm");
            var proc = program.Procedures.Values[0];
			BlockDominatorGraph doms = proc.CreateBlockDominatorGraph();
            Assert.IsTrue(doms.DominatesStrictly(proc.EntryBlock, proc.EntryBlock.Succ[0]));
            Assert.IsTrue(doms.DominatesStrictly(proc.EntryBlock, proc.EntryBlock.Succ[0].Succ[0]));
            Assert.IsTrue(doms.DominatesStrictly(proc.EntryBlock.Succ[0], proc.EntryBlock.Succ[0].Succ[0]));
            Assert.IsTrue(doms.DominatesStrictly(
                proc.EntryBlock.Succ[0].Succ[0].Succ[0],
                proc.EntryBlock.Succ[0].Succ[0].Succ[0].Succ[0]));
		}


		[Test]
		public void AnAliasExpanderTest()
		{
			Program program = RewriteFile("Fragments/alias_regs.asm");
			Procedure proc = program.Procedures.Values[0];
			Aliases alias = new Aliases(proc, program.Architecture);
			alias.Transform();
			using (FileUnitTester fut = new FileUnitTester("Analysis/AnAliasExpanderTest.txt"))
			{
				proc.Write(true, fut.TextWriter);
				fut.AssertFilesEqual();
			}
		}

		[Test]
		public void AnAliasExpandDeadVars()
		{
			Program program = RewriteFile("Fragments/alias_regs2.asm");
			Procedure proc = program.Procedures.Values[0];
			Aliases alias = new Aliases(proc, program.Architecture);
			alias.Transform();

			using (FileUnitTester fut = new FileUnitTester("Analysis/AnAliasExpandDeadVars.txt"))
			{
				proc.Write(true, fut.TextWriter);
				fut.AssertFilesEqual();
			}
		}
	}
}
