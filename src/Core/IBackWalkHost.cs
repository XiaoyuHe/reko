#region License
/* 
 * Copyright (C) 1999-2017 John K�ll�n.
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

using Reko.Core.Expressions;
using System;

namespace Reko.Core
{
	/// <summary>
	/// Interface used for backwalkers to get services from host.
	/// </summary>
	public interface IBackWalkHost
	{
		AddressRange GetSinglePredecessorAddressRange(Address block);
		Address GetBlockStartAddress(Address addr);
        Address MakeAddressFromConstant(Constant c);
        Address MakeSegmentedAddress(Constant selector, Constant offset);

        Block GetSinglePredecessor(Block block);

        bool IsValidAddress(Address addr);
        RegisterStorage GetSubregister(RegisterStorage rIdx, int v1, int v2);
    }
}
