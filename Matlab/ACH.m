
rng(5,'twister');

N = [57,58]; %���������� ����� (������ ��� �������������� � ������ �����), ������� �� ����� ����������� � ����.
rays = 9; %���������� �����
K = 1000;  %����� ����� k �������� ������� 
mu = zeros(rays, 2, K); %������� �������������

%������� ���������
%power = [0 -1 -2 -3 -8 -17.2 -20.8];
power = [-1,-1,-1,0,0,0,-3,-5,-7];
sigma = sqrt(10.^(power./10)); % �������� ���������

%����� ������������, �������� MEDS

%������� ������������ ��������
c = createGainsByMEDS(sigma, N(1));

%������� ������� ������������� ��������

%fray=800*10^6; %������� ����. (��)
%fmax = fray/(3*10^8); %������������ ������������ ��������
fmax = 300;
frec = createFrequenciesByMEDS(fmax, N(1));

% ������� ����
teta = createPhasesByMEDS(N(1));

%������� ������������� 
%Ts = 5000*10^(-6); 
Ts=1/5000;



for z=1:rays
    mu(z,:,:) = createRayCoeff(c(:,:,z),frec,teta,Ts,K,N);
end

ach = zeros(999,K);
%time = [0, 30, 70, 90, 110, 190, 410];
time = [0, 50, 120, 200, 230, 500, 1600, 2300, 5000];
for k=1:K
    for frec=1:999
        for z=1:rays
            h = mu(z,1,k)+1j*mu(z,1,k);
            ach(frec,k)=ach(frec,k)+h*exp(-1j*2*pi*15000*(frec-1)*time(z)*10^-9);
        end
    end
end

plot(erfc(abs(ach(:,1)).^2*0.5)); %����������� ������.
surf(ach(1:100,1:100).*conj(ach(1:100,1:100)));
%csv right. �������� ���� � �������� �������

%surf(real(ach));
