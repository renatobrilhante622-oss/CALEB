import * as T from 'three';
import {GLTFLoader} from 'three/addons/loaders/GLTFLoader.js';
export class AssetGallery {
 constructor(scene){this.mixers=[];this.scene=scene;this.loader=new GLTFLoader();this.load('RobotExpressive.glb',-5,6,.7);this.load('Parrot.glb',5,6,.035)}
 load(file,x,z,scale){this.loader.load('/assets/models/'+file,g=>{g.scene.position.set(x,file.startsWith('Parrot')?4:0,z);g.scene.scale.setScalar(scale);g.scene.traverse(o=>{if(o.isMesh)o.castShadow=true});this.scene.add(g.scene);const m=new T.AnimationMixer(g.scene);if(g.animations.length)m.clipAction(g.animations.find(a=>a.name==='Idle')||g.animations[0]).play();this.mixers.push(m)},undefined,e=>console.warn('Modelo local indisponível',file,e.message))}
 update(dt){this.mixers.forEach(m=>m.update(dt))}
}
