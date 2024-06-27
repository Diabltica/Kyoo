/*
 * Kyoo - A portable and vast media library solution.
 * Copyright (c) Kyoo.
 *
 * See AUTHORS.md and LICENSE file in the project root for full license information.
 *
 * Kyoo is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * any later version.
 *
 * Kyoo is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Kyoo. If not, see <https://www.gnu.org/licenses/>.
 */

import { Main } from "@kyoo/primitives";
import type { ReactElement } from "react";
import { useYoshiki, vw } from "yoshiki/native";
import { Navbar } from "./navbar";

export const DefaultLayout = ({
	page,
	transparent,
	gradient,
}: {
	page: ReactElement;
	transparent?: boolean;
	gradient?: boolean;
}) => {
	const { css } = useYoshiki();

	return (
		<>
			<Navbar
				{...css([
					transparent && {
						bg: "transparent",
						position: "absolute",
						top: 0,
						left: 0,
						right: 0,
						shadowOpacity: 0,
					},
					gradient && {
						background:
						`
						radial-gradient(at top left, rgba(0,0,0,0.8), rgba(0,0,0,0.0), rgba(0,0,0,0)),
						radial-gradient(at top right, rgba(0,0,0,0.8), rgba(0,0,0,0.0), rgba(0,0,0,0))
						`,					
						position: "absolute",
						top: 0,
						left: 0,
						right: 0,
						shadowOpacity: 0,
					},
				])}
			/>
			<Main
				{...css({
					display: "flex",
					width: vw(100),
					flexGrow: 1,
					flexShrink: 1,
					overflow: "hidden",
				})}
			>
				{page}
			</Main>
		</>
	);
};
