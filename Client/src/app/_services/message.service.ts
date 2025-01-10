import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { HttpClient } from '@angular/common/http';
import { PaginatedResult } from '../_models/paginations';
import { Message } from '../_models/message';
import { setPaginationHeaders, setPaginationResponse } from './paginationHelper';

@Injectable({
  providedIn: 'root',
})
export class MessageService {
  baseUrl = environment.apiUrl;
  private http = inject(HttpClient);
  paginatedResult = signal<PaginatedResult<Message[]> | null>(null);

  getMessages(pageNumber: number, pageSize: number, container: string){
    let params = setPaginationHeaders(pageNumber, pageSize);

    params = params.append('container', container);

    return this.http.get<Message[]>(this.baseUrl + 'messages', {observe: 'response', params: params})
      .subscribe({
        next: response => setPaginationResponse(response, this.paginatedResult)
      })
  }

  getMessageThread(username: string) {
    return this.http.get<Message[]>(this.baseUrl + 'messages/thread/' + username)
  }
  sendMessage(username: string, content: string){
    return this.http.post<Message>(this.baseUrl + 'messages', {recipientUsername: username, content: content})
  }
  deleteMessage(id: Number){
    return this.http.delete(this.baseUrl + 'messages/' + id);
  }
}
