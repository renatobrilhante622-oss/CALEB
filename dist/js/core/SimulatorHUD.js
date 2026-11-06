import * as T from 'three';
import {PETS} from '../builders/PetFactory.js';
export class SimulatorHUD{
 constructor(game){this.g=game;this.shown=game.save.data.coins;this.gems=game.save.data.diamonds||0;this.effects=[];
 const header=document.querySelector('header');const gem=document.createElement('span');gem.id='diamonds';header.insertBefore(gem,document.querySelector('#home'));
 const menu=document.createElement('nav');menu.id='side-menu';menu.setAttribute('aria-label','Menu do jogo');
 for(const [icon,title,action]of [['🐾','Pets',()=>this.pets()],['🎒','Inventário',()=>this.inventory()],['🗺️','Mapas',()=>document.querySelector('#launcher').hidden=false],['🌙','Modo calmo',()=>document.querySelector('#calm').click()],['👑','Presentes do Papai',()=>document.querySelector('#parents').click()]]){const b=document.createElement('button');b.textContent=icon;b.title=title;b.setAttribute('aria-label',title);b.onclick=action;menu.append(b)}document.body.append(menu);
 this.running=document.createElement('div');this.running.id='runner-badge';this.running.hidden=true;document.body.append(this.running);
 }
 pets(){const g=this.g;g.panel('<h2>🐾 Meus amigos</h2><div class="pet-grid" id="pet-grid"></div>');const grid=document.querySelector('#pet-grid');for(const pet of g.save.data.pets){const card=document.createElement('div');card.className='pet-card';const def=PETS[pet.id%40];card.innerHTML='<span>'+def.icon+'</span><strong>'+def.name+'</strong><small>⭐ '+pet.level+'</small>';grid.append(card)}if(!g.save.data.pets.length)grid.textContent='🥚 → 🐾';}
 inventory(){const s=this.g.save.data;this.g.panel('<h2>🎒 Tesouros</h2><div class="inventory-items"><span>🥚<strong>'+s.eggs+'</strong></span><span>💎<strong>'+(s.diamonds||0)+'</strong></span><span>🏡<strong>'+s.habitat+'/15</strong></span></div><p>🥚 → Incubadora dourada</p>')}
 fly(amount,position=this.g.player.position,kind='coin'){
 if(this.effects.length>14)return;const point=position.clone().add(new T.Vector3(0,2.7,0)).project(this.g.graphics.camera);if(point.z>1)return;
 const e=document.createElement('div');e.className='reward-fly '+(kind==='damage'?'damage':'');e.textContent=kind==='damage'?'💥 '+amount:'🪙 +'+amount;e.style.left=(point.x*.5+.5)*innerWidth+'px';e.style.top=(-point.y*.5+.5)*innerHeight+'px';document.body.append(e);
 const dest=document.querySelector('#coins').getBoundingClientRect(),start={x:(point.x*.5+.5)*innerWidth,y:(-point.y*.5+.5)*innerHeight};this.effects.push({e,start,end:{x:dest.x+dest.width/2,y:dest.y+dest.height/2},life:0,damage:kind==='damage'});
 }
 update(dt){const s=this.g.save.data;this.shown+=(s.coins-this.shown)*(1-Math.exp(-dt*12));document.querySelector('#coins').textContent='🪙 '+Math.round(this.shown).toLocaleString('pt-BR');document.querySelector('#diamonds').textContent='💎 '+(s.diamonds||0).toLocaleString('pt-BR');const near=this.g.map===0&&Math.hypot(this.g.player.position.x+10,this.g.player.position.z)<2;this.running.hidden=!near;this.running.textContent='⚡ ×'+[1,3,10,25,50,100][s.treadmill];
 this.effects=this.effects.filter(q=>{q.life+=dt;const t=Math.min(1,q.life/(s.calm?1.3:1)),u=1-t;const x=q.damage?q.start.x:q.start.x*u*u+(q.start.x+100)*2*u*t+q.end.x*t*t,y=q.damage?q.start.y-t*80:q.start.y*u*u+(q.start.y-150)*2*u*t+q.end.y*t*t;q.e.style.transform=`translate(${x-q.start.x}px,${y-q.start.y}px) scale(${1-t*.4})`;q.e.style.opacity=String(q.damage?1-t:t>.8?(1-t)*5:1);if(t===1){q.e.remove();return false}return true});
 }
}
