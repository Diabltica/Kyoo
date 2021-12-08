import { HTTP_INTERCEPTORS, HttpClientModule } from "@angular/common/http";
import { APP_INITIALIZER, NgModule } from "@angular/core";
import { MatButtonModule } from "@angular/material/button";
import { MatCardModule } from "@angular/material/card";
import { MatNativeDateModule, MatRippleModule } from "@angular/material/core";
import { MatIconModule } from "@angular/material/icon";
import { MatMenuModule } from "@angular/material/menu";
import { MatProgressBarModule } from "@angular/material/progress-bar";
import { MatSelectModule } from "@angular/material/select";
import { MatSliderModule } from "@angular/material/slider";
import { MatSnackBarModule } from "@angular/material/snack-bar";
import { MatTooltipModule } from "@angular/material/tooltip";
import { BrowserModule, HammerModule } from "@angular/platform-browser";
import { BrowserAnimationsModule } from "@angular/platform-browser/animations";
import { AppRoutingModule } from "./app-routing.module";
import { AppComponent } from "./app.component";
import { ItemsGridComponent } from "./components/items-grid/items-grid.component";
import { CollectionComponent } from "./pages/collection/collection.component";
import { EpisodesListComponent } from "./components/episodes-list/episodes-list.component";
import { NotFoundComponent } from "./pages/not-found/not-found.component";
import { PeopleListComponent } from "./components/people-list/people-list.component";
import {
	BufferToWidthPipe,
	FormatTimePipe,
	PlayerComponent, SupportedButtonPipe,
	VolumeToButtonPipe
} from "./pages/player/player.component";
import { SearchComponent } from "./pages/search/search.component";
import { ShowDetailsComponent } from "./pages/show-details/show-details.component";
import { FormsModule , ReactiveFormsModule } from "@angular/forms";
import { MatInputModule } from "@angular/material/input";
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatTabsModule } from "@angular/material/tabs";
import { PasswordValidator } from "./misc/password-validator";
import { MatCheckboxModule } from "@angular/material/checkbox";
import { MatDialogModule } from "@angular/material/dialog";
import { FallbackDirective, FallbackPipe } from "./misc/fallback.directive";
import { AuthModule } from "./auth/auth.module";
import { AuthRoutingModule } from "./auth/auth-routing.module";
import { TrailerDialogComponent } from "./pages/trailer-dialog/trailer-dialog.component";
import { ItemsListComponent } from "./components/items-list/items-list.component";
import { MetadataEditComponent } from "./pages/metadata-edit/metadata-edit.component";
import { MatChipsModule } from "@angular/material/chips";
import { MatAutocompleteModule } from "@angular/material/autocomplete";
import { MatExpansionModule } from "@angular/material/expansion";
import { InfiniteScrollModule } from "ngx-infinite-scroll";
import { ShowGridComponent } from "./components/show-grid/show-grid.component";
import { MatBadgeModule } from "@angular/material/badge";
import { StartupService } from "./services/startup.service";
import { LongPressDirective } from "./misc/long-press.directive";
import { DatetimeInterceptorService } from "./services/datetime-interceptor.service";
import { MatDatepickerModule } from "@angular/material/datepicker";


@NgModule({
	declarations: [
		AppComponent,
		NotFoundComponent,
		ItemsGridComponent,
		ShowDetailsComponent,
		EpisodesListComponent,
		PlayerComponent,
		CollectionComponent,
		SearchComponent,
		PeopleListComponent,
		PasswordValidator,
		FallbackDirective,
		TrailerDialogComponent,
		ItemsListComponent,
		MetadataEditComponent,
		ShowGridComponent,
		FormatTimePipe,
		BufferToWidthPipe,
		VolumeToButtonPipe,
		SupportedButtonPipe,
		LongPressDirective,
		FallbackPipe
	],
	imports: [
		BrowserModule,
		HttpClientModule,
		AuthRoutingModule,
		AppRoutingModule,
		BrowserAnimationsModule,
		MatSnackBarModule,
		MatProgressBarModule,
		MatButtonModule,
		MatIconModule,
		MatSelectModule,
		MatMenuModule,
		MatSliderModule,
		MatTooltipModule,
		MatRippleModule,
		MatCardModule,
		ReactiveFormsModule,
		MatInputModule,
		MatFormFieldModule,
		MatDialogModule,
		FormsModule,
		MatTabsModule,
		MatCheckboxModule,
		AuthModule,
		MatChipsModule,
		MatAutocompleteModule,
		MatExpansionModule,
		InfiniteScrollModule,
		MatBadgeModule,
		HammerModule,
		MatDatepickerModule,
		MatNativeDateModule
	],
	bootstrap: [AppComponent],
	exports: [
		FallbackDirective,
		FallbackPipe
	],
	providers: [
		StartupService,
		{
			provide: APP_INITIALIZER,
			useFactory: (startup: StartupService) => () => startup.load(),
			deps: [StartupService],
			multi: true
		},
		{
			provide: HTTP_INTERCEPTORS,
			useClass: DatetimeInterceptorService,
			multi: true
		}
	]
})
export class AppModule { }
