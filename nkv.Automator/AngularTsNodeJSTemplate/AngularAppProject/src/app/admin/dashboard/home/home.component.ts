import { Component, OnInit } from '@angular/core';
import { childRoutes } from '../../child-routes'
interface RouteModel {
  path: string;
  data: { icon: string, text: string };
}

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {
  tableList: RouteModel[] = [];
  bgColors: any[] = [];
  constructor() { }
  ngOnInit() {
    this.tableList = childRoutes;
    this.bgColors=['danger','warn','success','info']
  }
}
