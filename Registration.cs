        /* This is a large form registration API */
        
        public async Task<ServiceResponse<ClientRegistrationDto>> ClientRegistration(RegistrationDto Client)
        {
            ServiceResponse<ClientRegistrationDto> response = new ServiceResponse<RegistrationDto>();

            // Create List Variables from Client dto using ToList() method
            List<int> Prepare = Client.howToPrepareThingsToKnow.ToList();
            List<int> HealthAndSafety = Client.HealthAndSafetyThingsToKnow.ToList();
            List<int> Techniques = Client.TechniquesThingsToKnow.ToList();
            List<int> Miscellaneous = Client.MiscellaneousThingsToKnow.ToList();
            List<int> Special = Client.SpecialStuff.ToList();
            if (await ClientExists(Client.Name))
            {
                response.Success = false;
                response.Message = "Name already registered";
            }
            else
            {
                Client x = new Client()
                {
                    Name = Client.Name,
                    About = Client.AboutUs,
                    Business = Client.IsBusiness,
                    EstablishedDate = Client.EstablishedDate,
                    BookingDuration = (int)Client.AdvancedBookingDuration,
                    Demographic = Client.Demographic,
                    ContactPerson = Client.MainContactPerson,
                    ContactNo = Client.ContactNo,
                    Email = Client.Email,
                    Uen = Client.Uen,
                    Insurances = Client.InsurancesClaimable,
                    InsuranceCompanyName = Client.InsuranceCompanyName is not null ? string.Join(",", Client.InsuranceCompanyName) : null,
                    Created = DateTime.Now,
                    Special = new List<SpecialStuff>(),
                };
                if (Client.SpecialStuffOthers != null)
                {
                    //  add SpecialStuff - others
                    foreach (var s in Client.SpecialStuffOthers)
                    {
                        if (s.Trim().IsEmpty()) continue;
                        SpecialStuff existSpecialStuff = await _context.Special.SingleOrDefaultAsync(x => x.SpecialStuff.ToLower() == s.ToLower());
                        if ((existSpecialStuff == null))
                        {
                            Console.Write("new :" + s);
                            SpecialStuff newSpecialStuff = new()
                            {
                                SpecialStuff = s
                            };
                            await _context.Special.AddAsync(newSpecialStuff);
                            await _context.SaveChangesAsync();
                            x.Special.Add(newSpecialStuff);
                        }
                        else
                        { // in case user has the Special-others exist in Special entity
                            Console.Write("existing :" + s);
                            x.Special.Add(existSpecialStuff);
                        }
                    }
                }
                await _context.Clients.AddAsync(x);
                try
                {
                    await _context.SaveChangesAsync();
                    response.Message += "Client registered, ";
                }
                catch (Exception)
                {
                    response.Success = false;
                    response.Message += "Client registration error, ";
                    return response;
                }
                foreach (var SpecialStuff in specialitie)
                {
                    SpecialStuff specilityObj = await _context.Special.FindAsync(SpecialStuff);
                    x.Special.Add(specilityObj);
                }
                try
                {
                    await _context.SaveChangesAsync();
                    response.Message += "SpecialStuff id registered, ";
                }
                catch (Exception)
                {
                    response.Success = false;
                    response.Message += "SpecialStuff id error, ";
                    Console.WriteLine("SpecialStuff id error");
                    return response;
                }
                //Get Client id to pass in to ThingsToKnow
                x.ThingsToKnow = new ThingsToKnow()
                {
                    ClientId = x.Id,
                    Created = DateTime.Now,
                    HowToPrepare = new List<HowToPrepare>(),
                    HealthAndSafety = new List<HealthAndSafety>(),
                    Techniques = new List<Technique>(),
                    Miscellaneous = new List<Miscellaneous>()
                };
                await _context.ThingsToKnow.AddAsync(x.ThingsToKnow);
                try
                {
                    await _context.SaveChangesAsync();
                    response.Message += "ThingsToKnow registered, ";
                }
                catch (Exception)
                {
                    response.Success = false;
                    response.Message += "ThingsToKnow registration error, ";
                    return response;
                }
                //  add HowToPrepare - others
                if (Client.HowToPrepareOthers != null)
                {
                    foreach (var p in Client.HowToPrepareOthers)
                    {
                        if (!p.Trim().IsEmpty())
                        {
                            HowToPrepare existHowToPrepare = await _context.HowToPrepare.SingleOrDefaultAsync(x => x.Preparation.ToLower() == p.ToLower());
                            if (existHowToPrepare == null)
                            {
                                Console.Write("new :" + p);
                                HowToPrepare NewHowToPrepare = new()
                                {
                                    Preparation = p
                                };
                                await _context.HowToPrepare.AddAsync(NewHowToPrepare);
                                await _context.SaveChangesAsync();
                                x.ThingsToKnow.HowToPrepare.Add(NewHowToPrepare);
                            }
                            else
                            {
                                Console.Write("existing :" + p);
                                x.ThingsToKnow.HowToPrepare.Add(existHowToPrepare);
                            }
                        }
                    }
                }
                //  add HealthAndSafety - others
                if (Client.HealthAndSafetyOthers != null)
                {
                    foreach (var hs in Client.HealthAndSafetyOthers)
                    {
                        if (!hs.Trim().IsEmpty())
                        {
                            HealthAndSafety existHealthAndSafety = await _context.HealthAndSafety.SingleOrDefaultAsync(x => x.steps.ToLower() == hs.ToLower());

                            if (existHealthAndSafety == null)
                            {
                                Console.Write("new :" + hs);
                                HealthAndSafety NewHealthAndSafety = new()
                                {
                                    steps = hs
                                };
                                await _context.HealthAndSafety.AddAsync(NewHealthAndSafety);
                                await _context.SaveChangesAsync();
                                x.ThingsToKnow.HealthAndSafety.Add(NewHealthAndSafety);
                            }
                            else
                            {
                                Console.Write("existing :" + hs);
                                x.ThingsToKnow.HealthAndSafety.Add(existHealthAndSafety);
                            }
                        }
                    }
                }
                if (Client.TechniquesOthers != null)
                {
                    //  add Techniques - others
                    foreach (var v in Client.TechniquesOthers)
                    {
                        if (!v.Trim().IsEmpty())
                        {
                            Technique existTechniques = await _context.Techniques.SingleOrDefaultAsync(x => x.Techniques.ToLower() == v.ToLower());
                            if (existTechniques == null)
                            {
                                Console.Write("new :" + v);
                                Technique NewTechniques = new()
                                {
                                    Techniques = v
                                };
                                await _context.Techniques.AddAsync(NewTechniques);
                                await _context.SaveChangesAsync();
                                x.ThingsToKnow.Techniques.Add(NewTechniques);
                            }
                            else
                            {
                                Console.Write("existing :" + v);
                                x.ThingsToKnow.Techniques.Add(existTechniques);
                            }
                        }
                    }
                }
                if (Client.MiscellaneousOthers != null)
                {
                    //  add Miscellaneous - others
                    foreach (var m in Client.MiscellaneousOthers)
                    {
                        if (!m.Trim().IsEmpty())
                        {
                            Miscellaneous existMiscellaneous = await _context.Miscellaneous.SingleOrDefaultAsync(x => x.Misc.ToLower() == m.ToLower());
                            if (existMiscellaneous == null)
                            {
                                Console.Write("new :" + m);
                                Miscellaneous NewMiscellaneous = new()
                                {
                                    Misc = m
                                };
                                await _context.Miscellaneous.AddAsync(NewMiscellaneous);
                                await _context.SaveChangesAsync();

                                x.ThingsToKnow.Miscellaneous.Add(NewMiscellaneous);
                            }
                            else
                            {
                                Console.Write("existing :" + m);
                                x.ThingsToKnow.Miscellaneous.Add(existMiscellaneous);
                            }
                        }
                    }
                }
                try
                {
                    await _context.SaveChangesAsync();
                    response.Message += "ThingsToKnowOthers registered, ";
                }
                catch (Exception)
                {
                    response.Success = false;
                    response.Message += "ThingsToKnowOthers registration error, ";
                    return response;
                }
                //HowToPrepare
                foreach (var hw2prep in HowToPrepare)
                {
                    HowToPrepare hw2prepObj = await _context.HowToPrepare.FindAsync(hw2prep);
                    x.ThingsToKnow.HowToPrepare.Add(hw2prepObj);
                }
                await _context.SaveChangesAsync();
                //HealthAndSafety
                //_context.Entry(x.ThingsToKnow).Collection(x => x.HealthAndSafety).Load();
                foreach (var hns in HealthAndSafety)
                {
                    HealthAndSafety hnsObj = await _context.HealthAndSafety.FindAsync(hns);
                    x.ThingsToKnow.HealthAndSafety.Add(hnsObj);
                }
                await _context.SaveChangesAsync();
                //Techniques
                //_context.Entry(x.ThingsToKnow).Collection(x => x.Techniques).Load();
                foreach (var techq in Techniques)
                {
                    Technique techqObj = await _context.Techniques.FindAsync(techq);
                    x.ThingsToKnow.Techniques.Add(techqObj);
                }
                await _context.SaveChangesAsync();
                //Miscellaneous
                //_context.Entry(x.ThingsToKnow).Collection(x => x.Miscellaneous).Load();
                foreach (var misc in Miscellaneous)
                {
                    Miscellaneous miscObj = await _context.Miscellaneous.FindAsync(misc);
                    x.ThingsToKnow.Miscellaneous.Add(miscObj);
                }
                await _context.SaveChangesAsync();
                //ClientPayment
                ClientPayment y = new ClientPayment()
                {
                    AccountHolderName = Client.AccountHolderName,
                    BankAccName = Client.BankName + " " + Client.BankAccType,
                    BankAccNo = Client.BankAccNo,
                    ClientId = x.Id,
                };
                await _context.ClientPayments.AddAsync(y);
                try
                {
                    await _context.SaveChangesAsync();
                    response.Message += "ClientPayments registered, ";
                }
                catch (Exception)
                {
                    response.Success = false;
                    response.Message += "ClientPayments registration error, ";
                    Console.WriteLine("ClientPayments registration error");
                    return response;
                }

                response.Success = true;
                response.Message += "Client Registration part 1 successfully";

                response.AdditionalData = new Dictionary<string, object>
                {
                    { "ClientId", x.Id }
                };
            }

            return response;
        }