import { HttpClient, HttpContext } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { Member } from '../_models/member';
import { PaginatedResult } from '../_models/paginations';
import {
  setPaginationHeaders,
  setPaginationResponse,
} from './paginationHelper';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class LikesService {
  baseUrl = environment.apiUrl;
  private http = inject(HttpClient);
  likeIds = signal<number[]>([]);
  paginatedResult = signal<PaginatedResult<Member[]> | null>(null);

  toggleLike(targetId: number) {
    return this.http.post(this.baseUrl + 'likes/' + targetId, {});
  }
  getLikes(predicate: string, pageNumber: number, pageSize: number) {
    let params = setPaginationHeaders(pageNumber, pageSize);

    params = params.append('predicate', predicate);

    return this.http
      .get<Member[]>(this.baseUrl + 'likes', {
        observe: 'response',
        params: params,
      })
      .subscribe({
        next: (response) =>
          setPaginationResponse(response, this.paginatedResult),
      });
  }
  getLikeIds() {
    return this.http.get<number[]>(this.baseUrl + 'likes/list').subscribe({
      next: (ids) => this.likeIds.set(ids),
    });
  }
}
