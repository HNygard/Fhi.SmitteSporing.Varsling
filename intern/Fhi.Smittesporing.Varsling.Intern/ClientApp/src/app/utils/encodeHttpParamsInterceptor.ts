import { HttpInterceptor, HttpRequest, HttpEvent, HttpHandler, HttpParams, HttpParameterCodec } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { CustomHttpParamEncoder } from "./customHttpParamEncoder";

@Injectable()
export class EncodeHttpParamsInterceptor implements HttpInterceptor {
  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const params = new HttpParams({ encoder: new CustomHttpParamEncoder(), fromString: req.params.toString() });
    return next.handle(req.clone({ params }));
  }
}
