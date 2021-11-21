import { Season } from "./season";
import { Genre } from "./genre";
import { People } from "./people";
import { Studio } from "./studio";
import { ExternalID } from "../external-id";
import { IResource } from "./resource";

export interface Show extends IResource
{
	title: string;
	aliases: string[];
	overview: string;
	genres: Genre[];
	status: string;
	studio: Studio;
	people: People[];
	seasons: Season[];
	trailer: string;
	isMovie: boolean;
	startAir: Date;
	endAir: Date;
	poster: string;
	logo: string;
	thumbnail: string;

	externalIDs: ExternalID[];
}

export interface ShowRole extends IResource
{
	role: string;
	type: string;

	title: string;
	aliases: string[];
	overview: string;
	status: string;
	trailerUrl: string;
	isMovie: boolean;
	startAir: Date;
	endAir: Date;
	poster: string;
	logo: string;
	backdrop: string;
}
