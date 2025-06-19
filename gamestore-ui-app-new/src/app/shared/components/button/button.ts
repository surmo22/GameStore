import { Component, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-button',
  templateUrl: './button.html',
  styleUrls: ['./button.scss'],
  standalone: true
})
export class ButtonComponent {
  @Input() label = '';
  @Output() onClick = new EventEmitter<Event>();

  handleClick(event: Event) {
    this.onClick.emit(event);
  }
}
