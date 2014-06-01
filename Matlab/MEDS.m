N = [57,58]; %количество лучей (разное для действительной и мнимой части), которые мы будем суммировать в один.
rays = 7; %количество лучей
K = 10000;  %всего имеем k моментов времени 
mu = zeros(rays, 2, K); %таблица коэффициентов

%таблица мощностей
power = [0 -1 -2 -3 -8 -17.2 -20.8];
sigma = sqrt(10.^(power./10)); % вычислим дисперсию

%Задаём коэффициенты, согласно MEDS

%зададим коэффициенты усиления
c = createGainsByMEDS(sigma./sqrt(2), N(1));

%зададим частоту доплеровского смещения

%fray=800*10^6; %частота луча. (Гц)
%fmax = fray/(3*10^8); %Максимальное доплеровское смещение
fmax = 300;
f = createFrequenciesByMEDS(fmax, N(1));

% зададим фазы
teta = createPhasesByMEDS(N(1));

%Частота дискретизации 
%Ts = 5000*10^(-6); 
Ts=1/5000;



for z=1:rays
    mu(z,:,:) = createRayCoeff(c(:,:,z),f,teta,Ts,K,N);
end


srednee = zeros (7,1);
for s=1:7
    for k=1:K
        srednee(s)=srednee(s)+abs(mu(s,1,k)+1j*mu(s,2,k)).^2;
        
    end
    srednee(s)=srednee(s)/(K);
end
  
disp(srednee);
disp(sigma.^2);



 x=squeeze(mu(1,1,:)+1j*mu(1,2,:));

 M=30;
 R=correlation(x,M);

 x1=squeeze(mu(1,1,:));
 x2=squeeze(mu(1,2,:));


 hold off;
 plot(correlation(x1,M));
 hold on;
 plot(correlation(x2,M));
 plot(sigma(1)^2/2*besselj(0,2*pi*fmax*Ts*(0:M)),'r-.');