using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;

namespace ParkService.Models{
    public class Park{

        public Park(string _id){
            this._id = _id;
        }
        public string _id { get; set; }
        public string _rev {get; set; }
        public string Name { get; set; }
        public int Size {get; set;}
        public int Floors { get; set; }
        public int Rows { get; set; }
        public int Cells { get; set; }
        public float X {get; set;}
        public float Y {get; set;}
        public float Price {get; set;}
        public Cell[][][] Slots{get; set;}
        public string Owner_id{get;set;}
        public int FreeSpots(){
            int free=0;
            for(int i=0;i<Floors;i++){
                for(int j=0;j<Rows;j++){
                    for(int m=0;m<Cells;m++){
                        if (!Slots[i][j][m].statusCell())
                            free++;
                    }
                }
            }
            return free;
        }
    }
}