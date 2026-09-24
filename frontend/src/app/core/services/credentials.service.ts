import { Injectable } from '@angular/core';
import { HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { ApiResponse } from '../models/api-response.model';
import { CredentialListItem, CredentialDetail } from '../models/credential.model';
import { CreateCredentialRequest } from '../models/dtos/create-credential.request';
import { CreateCredentialResult } from '../models/dtos/create-credential.response';
import { MemberSearchResult } from '../models/dtos/member-search.response';
import { ActiveCredentialResult } from '../models/dtos/active-credential.response';

@Injectable({ providedIn: 'root' })
export class CredentialsService {
  constructor(private api: ApiService) {}

  list(): Observable<CredentialListItem[]> {
    return this.api.get<CredentialListItem[]>('credentials');
  }

  getById(id: string): Observable<ApiResponse<CredentialDetail>> {
    return this.api.get<ApiResponse<CredentialDetail>>(`credentials/${id}`);
  }

  create(request: CreateCredentialRequest): Observable<ApiResponse<CreateCredentialResult>> {
    return this.api.post<ApiResponse<CreateCredentialResult>>('credentials', request);
  }

  searchMembers(dniPrefix: string): Observable<ApiResponse<MemberSearchResult[]>> {
    const params = new HttpParams().set('dni', dniPrefix);
    return this.api.get<ApiResponse<MemberSearchResult[]>>('credentials/members/search', params);
  }

  getActiveCredential(dni: string): Observable<ApiResponse<ActiveCredentialResult | null>> {
    const params = new HttpParams().set('dni', dni);
    return this.api.get<ApiResponse<ActiveCredentialResult | null>>('credentials/members/active-credential', params);
  }
}
