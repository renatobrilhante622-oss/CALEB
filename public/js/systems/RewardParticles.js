import * as T from 'three';
export class RewardParticles{
 constructor(scene){const geo=new T.IcosahedronGeometry(.1,0),mat=new T.MeshBasicMaterial({color:0xffdb55});this.pool=Array.from({length:32},()=>{const m=new T.Mesh(geo,mat);m.visible=false;scene.add(m);return{m,life:0,v:new T.Vector3()}});this.cursor=0}
 emit(p,calm){for(let i=0;i<(calm?2:8);i++){const q=this.pool[this.cursor++%this.pool.length];q.life=.9;q.m.position.copy(p).add(new T.Vector3(0,1,0));q.m.visible=true;q.v.set((Math.random()-.5)*3,2+Math.random()*2,(Math.random()-.5)*3)}}
 update(dt){for(const q of this.pool)if(q.life>0){q.life-=dt;q.v.y-=4*dt;q.m.position.addScaledVector(q.v,dt);q.m.rotation.y+=dt*3;q.m.visible=q.life>0}}
}
