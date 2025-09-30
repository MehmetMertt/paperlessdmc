import { Routes } from '@angular/router';
import { RouterOutlet } from '@angular/router';
import { DocumentList} from './components/document-list/document-list';
import { Searchbar } from './components/searchbar/searchbar';
import { DocumentDetail } from './components/document-detail/document-detail';
import { Upload } from './components/upload/upload';
import { DocumentItem } from './components/document-list/document-list';
import { App } from './app';

export const routes: Routes = [ {path: 'api', component: App}];
