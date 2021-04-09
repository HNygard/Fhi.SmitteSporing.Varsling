import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'appDuration' })
export class DurationPipe implements PipeTransform {

  transform(value: any): string {
    var num = +value;
    if (typeof num === 'number') {
        const hours = Math.floor(num / 3600);
        const minutes = Math.floor((num % 3600) / 60);
        const seconds = Math.floor(num % 60);
        return `${hours}:${minutes < 10 ? '0' + minutes : minutes}:${seconds < 10 ? '0' + seconds :seconds}`;
    }
    return '';
  }
}